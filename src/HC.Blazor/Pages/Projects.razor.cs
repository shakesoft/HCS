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
using HC.Projects;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class Projects
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<ProjectWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<ProjectWithNavigationPropertiesDto> ProjectList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateProject { get; set; }

    private bool CanEditProject { get; set; }

    private bool CanDeleteProject { get; set; }

    private ProjectCreateDto NewProject { get; set; }

    private Validations NewProjectValidations { get; set; } = new();
    private ProjectUpdateDto EditingProject { get; set; }

    private Validations EditingProjectValidations { get; set; } = new();
    private Guid EditingProjectId { get; set; }

    private Modal CreateProjectModal { get; set; } = new();
    private Modal EditProjectModal { get; set; } = new();
    private GetProjectsInput Filter { get; set; }

    private DataGridEntityActionsColumn<ProjectWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "project-create-tab";
    protected string SelectedEditTab = "project-edit-tab";
    private ProjectWithNavigationPropertiesDto? SelectedProject;

    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<ProjectWithNavigationPropertiesDto> SelectedProjects { get; set; } = new();
    private bool AllProjectsSelected { get; set; }

    public Projects()
    {
        NewProject = new ProjectCreateDto();
        EditingProject = new ProjectUpdateDto();
        Filter = new GetProjectsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        ProjectList = new List<ProjectWithNavigationPropertiesDto>();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Projects"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewProject"], async () => {
            await OpenCreateProjectModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.Projects.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(ProjectWithNavigationPropertiesDto project)
    {
        DataGridRef.ToggleDetailRow(project, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<ProjectWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteProject;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<ProjectWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateProject = await AuthorizationService.IsGrantedAsync(HCPermissions.Projects.Create);
        CanEditProject = await AuthorizationService.IsGrantedAsync(HCPermissions.Projects.Edit);
        CanDeleteProject = await AuthorizationService.IsGrantedAsync(HCPermissions.Projects.Delete);
    }

    private async Task GetProjectsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await ProjectsAppService.GetListAsync(Filter);
        ProjectList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetProjectsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await ProjectsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/projects/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}&Description={HttpUtility.UrlEncode(Filter.Description)}&StartDateMin={Filter.StartDateMin?.ToString("O")}&StartDateMax={Filter.StartDateMax?.ToString("O")}&EndDateMin={Filter.EndDateMin?.ToString("O")}&EndDateMax={Filter.EndDateMax?.ToString("O")}&Status={HttpUtility.UrlEncode(Filter.Status)}&OwnerDepartmentId={Filter.OwnerDepartmentId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProjectWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetProjectsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateProjectModalAsync()
    {
        NewProject = new ProjectCreateDto
        {
            StartDate = DateTime.Now,
            EndDate = DateTime.Now,
        };
        SelectedCreateTab = "project-create-tab";
        await NewProjectValidations.ClearAll();
        await CreateProjectModal.Show();
    }

    private async Task CloseCreateProjectModalAsync()
    {
        NewProject = new ProjectCreateDto
        {
            StartDate = DateTime.Now,
            EndDate = DateTime.Now,
        };
        await CreateProjectModal.Hide();
    }

    private async Task OpenEditProjectModalAsync(ProjectWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "project-edit-tab";
        var project = await ProjectsAppService.GetWithNavigationPropertiesAsync(input.Project.Id);
        EditingProjectId = project.Project.Id;
        EditingProject = ObjectMapper.Map<ProjectDto, ProjectUpdateDto>(project.Project);
        await EditingProjectValidations.ClearAll();
        await EditProjectModal.Show();
    }

    private async Task DeleteProjectAsync(ProjectWithNavigationPropertiesDto input)
    {
        try
        {
            await ProjectsAppService.DeleteAsync(input.Project.Id);
            await GetProjectsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateProjectAsync()
    {
        try
        {
            if (await NewProjectValidations.ValidateAll() == false)
            {
                return;
            }

            await ProjectsAppService.CreateAsync(NewProject);
            await GetProjectsAsync();
            await CloseCreateProjectModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditProjectModalAsync()
    {
        await EditProjectModal.Hide();
    }

    private async Task UpdateProjectAsync()
    {
        try
        {
            if (await EditingProjectValidations.ValidateAll() == false)
            {
                return;
            }

            await ProjectsAppService.UpdateAsync(EditingProjectId, EditingProject);
            await GetProjectsAsync();
            await EditProjectModal.Hide();
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

    protected virtual async Task OnCodeChangedAsync(string? code)
    {
        Filter.Code = code;
        await SearchAsync();
    }

    protected virtual async Task OnNameChangedAsync(string? name)
    {
        Filter.Name = name;
        await SearchAsync();
    }

    protected virtual async Task OnDescriptionChangedAsync(string? description)
    {
        Filter.Description = description;
        await SearchAsync();
    }

    protected virtual async Task OnStartDateMinChangedAsync(DateTime? startDateMin)
    {
        Filter.StartDateMin = startDateMin.HasValue ? startDateMin.Value.Date : startDateMin;
        await SearchAsync();
    }

    protected virtual async Task OnStartDateMaxChangedAsync(DateTime? startDateMax)
    {
        Filter.StartDateMax = startDateMax.HasValue ? startDateMax.Value.Date.AddDays(1).AddSeconds(-1) : startDateMax;
        await SearchAsync();
    }

    protected virtual async Task OnEndDateMinChangedAsync(DateTime? endDateMin)
    {
        Filter.EndDateMin = endDateMin.HasValue ? endDateMin.Value.Date : endDateMin;
        await SearchAsync();
    }

    protected virtual async Task OnEndDateMaxChangedAsync(DateTime? endDateMax)
    {
        Filter.EndDateMax = endDateMax.HasValue ? endDateMax.Value.Date.AddDays(1).AddSeconds(-1) : endDateMax;
        await SearchAsync();
    }

    protected virtual async Task OnStatusChangedAsync(string? status)
    {
        Filter.Status = status;
        await SearchAsync();
    }

    protected virtual async Task OnOwnerDepartmentIdChangedAsync(Guid? ownerDepartmentId)
    {
        Filter.OwnerDepartmentId = ownerDepartmentId;
        await SearchAsync();
    }

    private async Task GetDepartmentCollectionLookupAsync(string? newValue = null)
    {
        DepartmentsCollection = (await ProjectsAppService.GetDepartmentLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllProjectsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllProjectsSelected = false;
        SelectedProjects.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedProjectRowsChanged()
    {
        if (SelectedProjects.Count != PageSize)
        {
            AllProjectsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedProjectsAsync()
    {
        var message = AllProjectsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedProjects.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllProjectsSelected)
        {
            await ProjectsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await ProjectsAppService.DeleteByIdsAsync(SelectedProjects.Select(x => x.Project.Id).ToList());
        }

        SelectedProjects.Clear();
        AllProjectsSelected = false;
        await GetProjectsAsync();
    }
}