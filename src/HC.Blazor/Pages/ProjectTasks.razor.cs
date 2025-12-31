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
using HC.ProjectTasks;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class ProjectTasks
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<ProjectTaskWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<ProjectTaskWithNavigationPropertiesDto> ProjectTaskList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateProjectTask { get; set; }

    private bool CanEditProjectTask { get; set; }

    private bool CanDeleteProjectTask { get; set; }

    private ProjectTaskCreateDto NewProjectTask { get; set; }

    private Validations NewProjectTaskValidations { get; set; } = new();
    private ProjectTaskUpdateDto EditingProjectTask { get; set; }

    private Validations EditingProjectTaskValidations { get; set; } = new();
    private Guid EditingProjectTaskId { get; set; }

    private Modal CreateProjectTaskModal { get; set; } = new();
    private Modal EditProjectTaskModal { get; set; } = new();
    private GetProjectTasksInput Filter { get; set; }

    private DataGridEntityActionsColumn<ProjectTaskWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "projectTask-create-tab";
    protected string SelectedEditTab = "projectTask-edit-tab";
    private ProjectTaskWithNavigationPropertiesDto? SelectedProjectTask;

    private IReadOnlyList<LookupDto<Guid>> ProjectsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<ProjectTaskWithNavigationPropertiesDto> SelectedProjectTasks { get; set; } = new();
    private bool AllProjectTasksSelected { get; set; }

    public ProjectTasks()
    {
        NewProjectTask = new ProjectTaskCreateDto();
        EditingProjectTask = new ProjectTaskUpdateDto();
        Filter = new GetProjectTasksInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        ProjectTaskList = new List<ProjectTaskWithNavigationPropertiesDto>();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["ProjectTasks"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewProjectTask"], async () => {
            await OpenCreateProjectTaskModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.ProjectTasks.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(ProjectTaskWithNavigationPropertiesDto projectTask)
    {
        DataGridRef.ToggleDetailRow(projectTask, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<ProjectTaskWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteProjectTask;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<ProjectTaskWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateProjectTask = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectTasks.Create);
        CanEditProjectTask = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectTasks.Edit);
        CanDeleteProjectTask = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectTasks.Delete);
    }

    private async Task GetProjectTasksAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await ProjectTasksAppService.GetListAsync(Filter);
        ProjectTaskList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetProjectTasksAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await ProjectTasksAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/project-tasks/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&ParentTaskId={HttpUtility.UrlEncode(Filter.ParentTaskId)}&Code={HttpUtility.UrlEncode(Filter.Code)}&Title={HttpUtility.UrlEncode(Filter.Title)}&Description={HttpUtility.UrlEncode(Filter.Description)}&StartDateMin={Filter.StartDateMin?.ToString("O")}&StartDateMax={Filter.StartDateMax?.ToString("O")}&DueDateMin={Filter.DueDateMin?.ToString("O")}&DueDateMax={Filter.DueDateMax?.ToString("O")}&Priority={HttpUtility.UrlEncode(Filter.Priority)}&Status={HttpUtility.UrlEncode(Filter.Status)}&ProgressPercentMin={Filter.ProgressPercentMin}&ProgressPercentMax={Filter.ProgressPercentMax}&ProjectId={Filter.ProjectId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProjectTaskWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetProjectTasksAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateProjectTaskModalAsync()
    {
        NewProjectTask = new ProjectTaskCreateDto
        {
            StartDate = DateTime.Now,
            DueDate = DateTime.Now,
        };
        SelectedCreateTab = "projectTask-create-tab";
        await NewProjectTaskValidations.ClearAll();
        await CreateProjectTaskModal.Show();
    }

    private async Task CloseCreateProjectTaskModalAsync()
    {
        NewProjectTask = new ProjectTaskCreateDto
        {
            StartDate = DateTime.Now,
            DueDate = DateTime.Now,
        };
        await CreateProjectTaskModal.Hide();
    }

    private async Task OpenEditProjectTaskModalAsync(ProjectTaskWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "projectTask-edit-tab";
        var projectTask = await ProjectTasksAppService.GetWithNavigationPropertiesAsync(input.ProjectTask.Id);
        EditingProjectTaskId = projectTask.ProjectTask.Id;
        EditingProjectTask = ObjectMapper.Map<ProjectTaskDto, ProjectTaskUpdateDto>(projectTask.ProjectTask);
        await EditingProjectTaskValidations.ClearAll();
        await EditProjectTaskModal.Show();
    }

    private async Task DeleteProjectTaskAsync(ProjectTaskWithNavigationPropertiesDto input)
    {
        try
        {
            await ProjectTasksAppService.DeleteAsync(input.ProjectTask.Id);
            await GetProjectTasksAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateProjectTaskAsync()
    {
        try
        {
            if (await NewProjectTaskValidations.ValidateAll() == false)
            {
                return;
            }

            await ProjectTasksAppService.CreateAsync(NewProjectTask);
            await GetProjectTasksAsync();
            await CloseCreateProjectTaskModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditProjectTaskModalAsync()
    {
        await EditProjectTaskModal.Hide();
    }

    private async Task UpdateProjectTaskAsync()
    {
        try
        {
            if (await EditingProjectTaskValidations.ValidateAll() == false)
            {
                return;
            }

            await ProjectTasksAppService.UpdateAsync(EditingProjectTaskId, EditingProjectTask);
            await GetProjectTasksAsync();
            await EditProjectTaskModal.Hide();
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

    protected virtual async Task OnParentTaskIdChangedAsync(string? parentTaskId)
    {
        Filter.ParentTaskId = parentTaskId;
        await SearchAsync();
    }

    protected virtual async Task OnCodeChangedAsync(string? code)
    {
        Filter.Code = code;
        await SearchAsync();
    }

    protected virtual async Task OnTitleChangedAsync(string? title)
    {
        Filter.Title = title;
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

    protected virtual async Task OnDueDateMinChangedAsync(DateTime? dueDateMin)
    {
        Filter.DueDateMin = dueDateMin.HasValue ? dueDateMin.Value.Date : dueDateMin;
        await SearchAsync();
    }

    protected virtual async Task OnDueDateMaxChangedAsync(DateTime? dueDateMax)
    {
        Filter.DueDateMax = dueDateMax.HasValue ? dueDateMax.Value.Date.AddDays(1).AddSeconds(-1) : dueDateMax;
        await SearchAsync();
    }

    protected virtual async Task OnPriorityChangedAsync(string? priority)
    {
        Filter.Priority = priority;
        await SearchAsync();
    }

    protected virtual async Task OnStatusChangedAsync(string? status)
    {
        Filter.Status = status;
        await SearchAsync();
    }

    protected virtual async Task OnProgressPercentMinChangedAsync(int? progressPercentMin)
    {
        Filter.ProgressPercentMin = progressPercentMin;
        await SearchAsync();
    }

    protected virtual async Task OnProgressPercentMaxChangedAsync(int? progressPercentMax)
    {
        Filter.ProgressPercentMax = progressPercentMax;
        await SearchAsync();
    }

    protected virtual async Task OnProjectIdChangedAsync(Guid? projectId)
    {
        Filter.ProjectId = projectId;
        await SearchAsync();
    }

    private async Task GetProjectCollectionLookupAsync(string? newValue = null)
    {
        ProjectsCollection = (await ProjectTasksAppService.GetProjectLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllProjectTasksSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllProjectTasksSelected = false;
        SelectedProjectTasks.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedProjectTaskRowsChanged()
    {
        if (SelectedProjectTasks.Count != PageSize)
        {
            AllProjectTasksSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedProjectTasksAsync()
    {
        var message = AllProjectTasksSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedProjectTasks.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllProjectTasksSelected)
        {
            await ProjectTasksAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await ProjectTasksAppService.DeleteByIdsAsync(SelectedProjectTasks.Select(x => x.ProjectTask.Id).ToList());
        }

        SelectedProjectTasks.Clear();
        AllProjectTasksSelected = false;
        await GetProjectTasksAsync();
    }
}