using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using HC.ProjectMembers;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class ProjectMembers
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<ProjectMemberWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<ProjectMemberWithNavigationPropertiesDto> ProjectMemberList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateProjectMember { get; set; }

    private bool CanEditProjectMember { get; set; }

    private bool CanDeleteProjectMember { get; set; }

    private ProjectMemberCreateDto NewProjectMember { get; set; }

    private Validations NewProjectMemberValidations { get; set; } = new();
    private ProjectMemberUpdateDto EditingProjectMember { get; set; }

    private Validations EditingProjectMemberValidations { get; set; } = new();
    private Guid EditingProjectMemberId { get; set; }

    private Modal CreateProjectMemberModal { get; set; } = new();
    private Modal EditProjectMemberModal { get; set; } = new();
    private GetProjectMembersInput Filter { get; set; }

    private DataGridEntityActionsColumn<ProjectMemberWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "projectMember-create-tab";
    protected string SelectedEditTab = "projectMember-edit-tab";
    private ProjectMemberWithNavigationPropertiesDto? SelectedProjectMember;

    private IReadOnlyList<LookupDto<Guid>> ProjectsCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<ProjectMemberWithNavigationPropertiesDto> SelectedProjectMembers { get; set; } = new();
    private bool AllProjectMembersSelected { get; set; }

    public ProjectMembers()
    {
        NewProjectMember = new ProjectMemberCreateDto();
        EditingProjectMember = new ProjectMemberUpdateDto();
        Filter = new GetProjectMembersInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        ProjectMemberList = new List<ProjectMemberWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["ProjectMembers"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewProjectMember"], async () => {
            await OpenCreateProjectMemberModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.ProjectMembers.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(ProjectMemberWithNavigationPropertiesDto projectMember)
    {
        DataGridRef.ToggleDetailRow(projectMember, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<ProjectMemberWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteProjectMember;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<ProjectMemberWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateProjectMember = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectMembers.Create);
        CanEditProjectMember = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectMembers.Edit);
        CanDeleteProjectMember = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectMembers.Delete);
    }

    private async Task GetProjectMembersAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await ProjectMembersAppService.GetListAsync(Filter);
        ProjectMemberList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetProjectMembersAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await ProjectMembersAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/project-members/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&MemberRole={HttpUtility.UrlEncode(Filter.MemberRole)}&JoinedAtMin={Filter.JoinedAtMin?.ToString("O")}&JoinedAtMax={Filter.JoinedAtMax?.ToString("O")}&ProjectId={Filter.ProjectId}&UserId={Filter.UserId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProjectMemberWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetProjectMembersAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateProjectMemberModalAsync()
    {
        NewProjectMember = new ProjectMemberCreateDto
        {
            JoinedAt = DateTime.Now,
        };
        SelectedCreateTab = "projectMember-create-tab";
        await NewProjectMemberValidations.ClearAll();
        await CreateProjectMemberModal.Show();
    }

    private async Task CloseCreateProjectMemberModalAsync()
    {
        NewProjectMember = new ProjectMemberCreateDto
        {
            JoinedAt = DateTime.Now,
        };
        await CreateProjectMemberModal.Hide();
    }

    private async Task OpenEditProjectMemberModalAsync(ProjectMemberWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "projectMember-edit-tab";
        var projectMember = await ProjectMembersAppService.GetWithNavigationPropertiesAsync(input.ProjectMember.Id);
        EditingProjectMemberId = projectMember.ProjectMember.Id;
        EditingProjectMember = ObjectMapper.Map<ProjectMemberDto, ProjectMemberUpdateDto>(projectMember.ProjectMember);
        await EditingProjectMemberValidations.ClearAll();
        await EditProjectMemberModal.Show();
    }

    private async Task DeleteProjectMemberAsync(ProjectMemberWithNavigationPropertiesDto input)
    {
        try
        {
            await ProjectMembersAppService.DeleteAsync(input.ProjectMember.Id);
            await GetProjectMembersAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateProjectMemberAsync()
    {
        try
        {
            if (await NewProjectMemberValidations.ValidateAll() == false)
            {
                return;
            }

            await ProjectMembersAppService.CreateAsync(NewProjectMember);
            await GetProjectMembersAsync();
            await CloseCreateProjectMemberModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditProjectMemberModalAsync()
    {
        await EditProjectMemberModal.Hide();
    }

    private async Task UpdateProjectMemberAsync()
    {
        try
        {
            if (await EditingProjectMemberValidations.ValidateAll() == false)
            {
                return;
            }

            await ProjectMembersAppService.UpdateAsync(EditingProjectMemberId, EditingProjectMember);
            await GetProjectMembersAsync();
            await EditProjectMemberModal.Hide();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void OnSelectedCreateTabChanged(string name)
    {
        SelectedCreateTab = name;
    }

    private void OnSelectedEditTabChanged(string name)
    {
        SelectedEditTab = name;
    }

    protected virtual async Task OnMemberRoleChangedAsync(string? memberRole)
    {
        Filter.MemberRole = memberRole;
        await SearchAsync();
    }

    protected virtual async Task OnJoinedAtMinChangedAsync(DateTime? joinedAtMin)
    {
        Filter.JoinedAtMin = joinedAtMin.HasValue ? joinedAtMin.Value.Date : joinedAtMin;
        await SearchAsync();
    }

    protected virtual async Task OnJoinedAtMaxChangedAsync(DateTime? joinedAtMax)
    {
        Filter.JoinedAtMax = joinedAtMax.HasValue ? joinedAtMax.Value.Date.AddDays(1).AddSeconds(-1) : joinedAtMax;
        await SearchAsync();
    }

    protected virtual async Task OnProjectIdChangedAsync(Guid? projectId)
    {
        Filter.ProjectId = projectId;
        await SearchAsync();
    }

    protected virtual async Task OnUserIdChangedAsync(Guid? userId)
    {
        Filter.UserId = userId;
        await SearchAsync();
    }

    private async Task GetProjectCollectionLookupAsync(string? newValue = null)
    {
        ProjectsCollection = (await ProjectMembersAppService.GetProjectLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
    {
        IdentityUsersCollection = (await ProjectMembersAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllProjectMembersSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllProjectMembersSelected = false;
        SelectedProjectMembers.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedProjectMemberRowsChanged()
    {
        if (SelectedProjectMembers.Count != PageSize)
        {
            AllProjectMembersSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedProjectMembersAsync()
    {
        var message = AllProjectMembersSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedProjectMembers.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllProjectMembersSelected)
        {
            await ProjectMembersAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await ProjectMembersAppService.DeleteByIdsAsync(SelectedProjectMembers.Select(x => x.ProjectMember.Id).ToList());
        }

        SelectedProjectMembers.Clear();
        AllProjectMembersSelected = false;
        await GetProjectMembersAsync();
    }
}