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
using HC.ProjectTaskAssignments;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class ProjectTaskAssignments
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<ProjectTaskAssignmentWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<ProjectTaskAssignmentWithNavigationPropertiesDto> ProjectTaskAssignmentList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateProjectTaskAssignment { get; set; }

    private bool CanEditProjectTaskAssignment { get; set; }

    private bool CanDeleteProjectTaskAssignment { get; set; }

    private ProjectTaskAssignmentCreateDto NewProjectTaskAssignment { get; set; }

    private Validations NewProjectTaskAssignmentValidations { get; set; } = new();
    private ProjectTaskAssignmentUpdateDto EditingProjectTaskAssignment { get; set; }

    private Validations EditingProjectTaskAssignmentValidations { get; set; } = new();
    private Guid EditingProjectTaskAssignmentId { get; set; }

    private Modal CreateProjectTaskAssignmentModal { get; set; } = new();
    private Modal EditProjectTaskAssignmentModal { get; set; } = new();
    private GetProjectTaskAssignmentsInput Filter { get; set; }

    private DataGridEntityActionsColumn<ProjectTaskAssignmentWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "projectTaskAssignment-create-tab";
    protected string SelectedEditTab = "projectTaskAssignment-edit-tab";
    private ProjectTaskAssignmentWithNavigationPropertiesDto? SelectedProjectTaskAssignment;

    private IReadOnlyList<LookupDto<Guid>> ProjectTasksCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<ProjectTaskAssignmentWithNavigationPropertiesDto> SelectedProjectTaskAssignments { get; set; } = new();
    private bool AllProjectTaskAssignmentsSelected { get; set; }

    public ProjectTaskAssignments()
    {
        NewProjectTaskAssignment = new ProjectTaskAssignmentCreateDto();
        EditingProjectTaskAssignment = new ProjectTaskAssignmentUpdateDto();
        Filter = new GetProjectTaskAssignmentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        ProjectTaskAssignmentList = new List<ProjectTaskAssignmentWithNavigationPropertiesDto>();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["ProjectTaskAssignments"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewProjectTaskAssignment"], async () => {
            await OpenCreateProjectTaskAssignmentModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.ProjectTaskAssignments.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(ProjectTaskAssignmentWithNavigationPropertiesDto projectTaskAssignment)
    {
        DataGridRef.ToggleDetailRow(projectTaskAssignment, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<ProjectTaskAssignmentWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteProjectTaskAssignment;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<ProjectTaskAssignmentWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateProjectTaskAssignment = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectTaskAssignments.Create);
        CanEditProjectTaskAssignment = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectTaskAssignments.Edit);
        CanDeleteProjectTaskAssignment = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectTaskAssignments.Delete);
    }

    private async Task GetProjectTaskAssignmentsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await ProjectTaskAssignmentsAppService.GetListAsync(Filter);
        ProjectTaskAssignmentList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetProjectTaskAssignmentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await ProjectTaskAssignmentsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/project-task-assignments/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&AssignmentRole={HttpUtility.UrlEncode(Filter.AssignmentRole)}&AssignedAtMin={Filter.AssignedAtMin?.ToString("O")}&AssignedAtMax={Filter.AssignedAtMax?.ToString("O")}&Note={HttpUtility.UrlEncode(Filter.Note)}&ProjectTaskId={Filter.ProjectTaskId}&UserId={Filter.UserId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProjectTaskAssignmentWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetProjectTaskAssignmentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateProjectTaskAssignmentModalAsync()
    {
        NewProjectTaskAssignment = new ProjectTaskAssignmentCreateDto
        {
            AssignedAt = DateTime.Now,
        };
        SelectedCreateTab = "projectTaskAssignment-create-tab";
        await NewProjectTaskAssignmentValidations.ClearAll();
        await CreateProjectTaskAssignmentModal.Show();
    }

    private async Task CloseCreateProjectTaskAssignmentModalAsync()
    {
        NewProjectTaskAssignment = new ProjectTaskAssignmentCreateDto
        {
            AssignedAt = DateTime.Now,
        };
        await CreateProjectTaskAssignmentModal.Hide();
    }

    private async Task OpenEditProjectTaskAssignmentModalAsync(ProjectTaskAssignmentWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "projectTaskAssignment-edit-tab";
        var projectTaskAssignment = await ProjectTaskAssignmentsAppService.GetWithNavigationPropertiesAsync(input.ProjectTaskAssignment.Id);
        EditingProjectTaskAssignmentId = projectTaskAssignment.ProjectTaskAssignment.Id;
        EditingProjectTaskAssignment = ObjectMapper.Map<ProjectTaskAssignmentDto, ProjectTaskAssignmentUpdateDto>(projectTaskAssignment.ProjectTaskAssignment);
        await EditingProjectTaskAssignmentValidations.ClearAll();
        await EditProjectTaskAssignmentModal.Show();
    }

    private async Task DeleteProjectTaskAssignmentAsync(ProjectTaskAssignmentWithNavigationPropertiesDto input)
    {
        try
        {
            await ProjectTaskAssignmentsAppService.DeleteAsync(input.ProjectTaskAssignment.Id);
            await GetProjectTaskAssignmentsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateProjectTaskAssignmentAsync()
    {
        try
        {
            if (await NewProjectTaskAssignmentValidations.ValidateAll() == false)
            {
                return;
            }

            await ProjectTaskAssignmentsAppService.CreateAsync(NewProjectTaskAssignment);
            await GetProjectTaskAssignmentsAsync();
            await CloseCreateProjectTaskAssignmentModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditProjectTaskAssignmentModalAsync()
    {
        await EditProjectTaskAssignmentModal.Hide();
    }

    private async Task UpdateProjectTaskAssignmentAsync()
    {
        try
        {
            if (await EditingProjectTaskAssignmentValidations.ValidateAll() == false)
            {
                return;
            }

            await ProjectTaskAssignmentsAppService.UpdateAsync(EditingProjectTaskAssignmentId, EditingProjectTaskAssignment);
            await GetProjectTaskAssignmentsAsync();
            await EditProjectTaskAssignmentModal.Hide();
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

    protected virtual async Task OnAssignmentRoleChangedAsync(string? assignmentRole)
    {
        Filter.AssignmentRole = assignmentRole;
        await SearchAsync();
    }

    protected virtual async Task OnAssignedAtMinChangedAsync(DateTime? assignedAtMin)
    {
        Filter.AssignedAtMin = assignedAtMin.HasValue ? assignedAtMin.Value.Date : assignedAtMin;
        await SearchAsync();
    }

    protected virtual async Task OnAssignedAtMaxChangedAsync(DateTime? assignedAtMax)
    {
        Filter.AssignedAtMax = assignedAtMax.HasValue ? assignedAtMax.Value.Date.AddDays(1).AddSeconds(-1) : assignedAtMax;
        await SearchAsync();
    }

    protected virtual async Task OnNoteChangedAsync(string? note)
    {
        Filter.Note = note;
        await SearchAsync();
    }

    protected virtual async Task OnProjectTaskIdChangedAsync(Guid? projectTaskId)
    {
        Filter.ProjectTaskId = projectTaskId;
        await SearchAsync();
    }

    protected virtual async Task OnUserIdChangedAsync(Guid? userId)
    {
        Filter.UserId = userId;
        await SearchAsync();
    }

    private async Task GetProjectTaskCollectionLookupAsync(string? newValue = null)
    {
        ProjectTasksCollection = (await ProjectTaskAssignmentsAppService.GetProjectTaskLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
    {
        IdentityUsersCollection = (await ProjectTaskAssignmentsAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllProjectTaskAssignmentsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllProjectTaskAssignmentsSelected = false;
        SelectedProjectTaskAssignments.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedProjectTaskAssignmentRowsChanged()
    {
        if (SelectedProjectTaskAssignments.Count != PageSize)
        {
            AllProjectTaskAssignmentsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedProjectTaskAssignmentsAsync()
    {
        var message = AllProjectTaskAssignmentsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedProjectTaskAssignments.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllProjectTaskAssignmentsSelected)
        {
            await ProjectTaskAssignmentsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await ProjectTaskAssignmentsAppService.DeleteByIdsAsync(SelectedProjectTaskAssignments.Select(x => x.ProjectTaskAssignment.Id).ToList());
        }

        SelectedProjectTaskAssignments.Clear();
        AllProjectTaskAssignmentsSelected = false;
        await GetProjectTaskAssignmentsAsync();
    }
}