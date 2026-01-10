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
using HC.ProjectTaskAssignments;
using HC.ProjectTaskDocuments;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;
using System.Threading;
using Volo.Abp.Identity;

namespace HC.Blazor.Pages;

public partial class ProjectTasks
{
    [Inject] private IProjectTaskAssignmentsAppService ProjectTaskAssignmentsAppService { get; set; } = default!;
    [Inject] private IProjectTaskDocumentsAppService ProjectTaskDocumentsAppService { get; set; } = default!;

    // Kanban UI
    protected bool IsKanbanView { get; set; } = true;
    protected bool ShowCancelledLane { get; set; }
    private bool IsKanbanLoadedOnce { get; set; }
    protected int KanbanRenderKey { get; set; }
    protected bool IsKanbanUpdating { get; set; }

    private const int KanbanMaxResultCount = 1000;

    protected sealed class KanbanItem
    {
        public Guid Id { get; init; }
        public string ProjectName { get; set; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectTaskPriority Priority { get; set; } = ProjectTaskPriority.LOW;
        public DateTime? DueDate { get; set; }
        public ProjectTaskStatus Status { get; set; }
        public int ProgressPercent { get; set; }
        public List<IdentityUserDto> Assignees { get; set; } = new();
        public int DocumentsCount { get; set; }

        // Keep the full DTO so we can update via AppService.
        public ProjectTaskDto ProjectTask { get; init; } = null!;

        // Keep navigation DTO for edit/delete actions from Kanban card.
        public ProjectTaskWithNavigationPropertiesDto ProjectTaskWithNavigationProperties { get; init; } = null!;
    }

    protected List<KanbanItem> KanbanItems { get; set; } = new();

    // Create task modal helpers (Select2 + Enum selects)
    private List<LookupDto<Guid>> SelectedNewProjectTaskProject { get; set; } = new();

    protected sealed class ParentTaskSelectItem
    {
        public string Id { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
    }

    private IReadOnlyList<ParentTaskSelectItem> ParentTasksCollection { get; set; } = new List<ParentTaskSelectItem>();
    private List<ParentTaskSelectItem> SelectedNewProjectTaskParentTask { get; set; } = new();

    private ProjectTaskPriority NewProjectTaskPriority { get; set; } = ProjectTaskPriority.LOW;
    private ProjectTaskStatus NewProjectTaskStatus { get; set; } = ProjectTaskStatus.TODO;

    private DatePicker<DateTime>? NewProjectTaskStartDateDatePicker { get; set; }
    private DatePicker<DateTime>? NewProjectTaskDueDateDatePicker { get; set; }

    // Create wizard state (General -> Assignments -> Documents)
    private Guid CreateWizardProjectTaskId { get; set; }
    protected bool IsCreateWizardGeneralSaved => CreateWizardProjectTaskId != Guid.Empty;
    private string? CreateGeneralValidationErrorKey { get; set; }

    // Assignments (create wizard)
    private IReadOnlyList<ProjectTaskAssignmentWithNavigationPropertiesDto> CreateAssignmentsList { get; set; } = new List<ProjectTaskAssignmentWithNavigationPropertiesDto>();
    private IReadOnlyList<LookupDto<Guid>> AssignmentIdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<LookupDto<Guid>> CreateAssignmentsUsersToAdd { get; set; } = new();
    private ProjectTaskAssignmentRole CreateAssignmentRole { get; set; } = ProjectTaskAssignmentRole.MAIN;
    private string? CreateAssignmentNote { get; set; }

    // Documents (create wizard)
    private IReadOnlyList<ProjectTaskDocumentWithNavigationPropertiesDto> CreateDocumentsList { get; set; } = new List<ProjectTaskDocumentWithNavigationPropertiesDto>();
    private IReadOnlyList<LookupDto<Guid>> DocumentsLookupCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<LookupDto<Guid>> CreateDocumentsToAdd { get; set; } = new();
    private ProjectTaskDocumentPurpose CreateDocumentPurpose { get; set; } = ProjectTaskDocumentPurpose.REPORT;

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; set;} = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<ProjectTaskWithNavigationPropertiesDto>? DataGridRef { get; set; }

    private IReadOnlyList<ProjectTaskWithNavigationPropertiesDto> ProjectTaskList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateProjectTask { get; set; }

    private bool CanEditProjectTask { get; set; }

    private bool CanDeleteProjectTask { get; set; }

    private ProjectTaskDto NewProjectTask { get; set; }

    private Validations NewProjectTaskValidations { get; set; } = new();
    private ProjectTaskUpdateDto EditingProjectTask { get; set; }

    private Validations EditingProjectTaskValidations { get; set; } = new();
    private Guid EditingProjectTaskId { get; set; }

    private Modal CreateProjectTaskModal { get; set; } = new();
    private Modal EditProjectTaskModal { get; set; } = new();
    private GetProjectTasksInput Filter { get; set; }

    private DataGridEntityActionsColumn<ProjectTaskWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "general";
    protected string SelectedEditTab = "projectTask-edit-tab";
    private ProjectTaskWithNavigationPropertiesDto? SelectedProjectTask;

    private IReadOnlyList<LookupDto<Guid>> ProjectsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<ProjectTaskWithNavigationPropertiesDto> SelectedProjectTasks { get; set; } = new();
    private bool AllProjectTasksSelected { get; set; }

    public ProjectTasks()
    {
        NewProjectTask = new ProjectTaskDto();
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

        await GetProjectCollectionLookupAsync();
        await RefreshKanbanAsync();
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
        RebuildToolbar();
        return ValueTask.CompletedTask;
    }

    private void RebuildToolbar()
    {
        Toolbar = new PageToolbar();
        Toolbar.AddButton(IsKanbanView ? L["List"] : L["Kanban"], async () => { await ToggleViewAsync(); }, IsKanbanView ? IconName.List : IconName.GripVertical);

        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);

        Toolbar.AddButton(L["NewTask"], async () => {
            await OpenCreateProjectTaskModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.ProjectTasks.Create);
    }

    private async Task ToggleViewAsync()
    {
        if (IsKanbanView)
        {
            IsKanbanView = false;
            await GetProjectTasksAsync();
        }
        else
        {
            IsKanbanView = true;
            await RefreshKanbanAsync();
        }

        RebuildToolbar();
        await InvokeAsync(StateHasChanged);
    }

    private void ToggleDetails(ProjectTaskWithNavigationPropertiesDto projectTask)
    {
        DataGridRef?.ToggleDetailRow(projectTask, true);
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

    protected string GetStatusText(ProjectTaskStatus status)
    {
        // Uses ABP localization keys defined in en.json/vi.json.
        return L[$"Enum:ProjectTaskStatus.{status}"];
    }

    protected int GetKanbanCount(ProjectTaskStatus status)
    {
        return KanbanItems.Count(x => x.Status == status);
    }

    protected Color GetPriorityBadgeColor(ProjectTaskPriority priority)
    {
        return priority switch
        {
            ProjectTaskPriority.LOW => Color.Secondary,
            ProjectTaskPriority.MEDIUM => Color.Info,
            ProjectTaskPriority.HIGH => Color.Warning,
            ProjectTaskPriority.URGENT => Color.Danger,
            _ => Color.Secondary,
        };
    }

    protected Color GetStatusBadgeColor(ProjectTaskStatus status)
    {
        return status switch
        {
            ProjectTaskStatus.TODO => Color.Secondary,
            ProjectTaskStatus.IN_PROGRESS => Color.Primary,
            ProjectTaskStatus.WAITING => Color.Warning,
            ProjectTaskStatus.DONE => Color.Success,
            ProjectTaskStatus.CANCELLED => Color.Danger,
            _ => Color.Secondary,
        };
    }

    
    protected Color GetPercentBadgeColor(int progressPercent)
    {
        return progressPercent switch
        {
            < 30 => Color.Danger,
            >= 30 and < 75 => Color.Warning,
            >= 75 and < 100 => Color.Primary,
            >= 100 => Color.Success
        };
    }

    protected string GetPriorityText(ProjectTaskPriority priority)
    {
        return L[$"Enum:ProjectTaskPriority.{priority}"];
    }

    protected Task OnKanbanItemDropped(DraggableDroppedEventArgs<KanbanItem> args)
    {
        return OnKanbanItemDroppedAsync(args);
    }

    private async Task OnKanbanItemDroppedAsync(DraggableDroppedEventArgs<KanbanItem> args)
    {
        if (args.Item is null)
        {
            return;
        }

        if (!Enum.TryParse<ProjectTaskStatus>(args.DropZoneName, ignoreCase: true, out var newStatus))
        {
            return;
        }

        if (args.Item.Status == newStatus)
        {
            return;
        }

        IsKanbanUpdating = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            await UpdateProjectTaskStatusAsync(args.Item, newStatus);
            await GetProjectTasksAsync();
            await RefreshKanbanAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsKanbanUpdating = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task UpdateProjectTaskStatusAsync(KanbanItem item, ProjectTaskStatus newStatus)
    {
        var input = new ProjectTaskUpdateDto
        {
            ParentTaskId = item.ProjectTask.ParentTaskId,
            Code = item.ProjectTask.Code,
            Title = item.ProjectTask.Title,
            Description = item.ProjectTask.Description,
            StartDate = item.ProjectTask.StartDate,
            DueDate = item.ProjectTask.DueDate,
            Priority = item.ProjectTask.Priority,
            Status = newStatus.ToString(),
            ProgressPercent = item.ProjectTask.ProgressPercent,
            ProjectId = item.ProjectTask.ProjectId,
            ConcurrencyStamp = item.ProjectTask.ConcurrencyStamp
        };

        await ProjectTasksAppService.UpdateAsync(item.ProjectTask.Id, input);

        // Update local state after the server call succeeds.
        item.ProjectTask.Status = input.Status;
        item.Status = newStatus;
    }

    private async Task RefreshKanbanAsync()
    {
        var input = new GetProjectTasksInput
        {
            FilterText = Filter.FilterText,
            ParentTaskId = Filter.ParentTaskId,
            Code = Filter.Code,
            Title = Filter.Title,
            Description = Filter.Description,
            StartDateMin = Filter.StartDateMin,
            StartDateMax = Filter.StartDateMax,
            DueDateMin = Filter.DueDateMin,
            DueDateMax = Filter.DueDateMax,
            Priority = Filter.Priority,
            Status = Filter.Status,
            ProgressPercentMin = Filter.ProgressPercentMin,
            ProgressPercentMax = Filter.ProgressPercentMax,
            ProjectId = Filter.ProjectId,
            SkipCount = 0,
            MaxResultCount = KanbanMaxResultCount,
            Sorting = string.Empty
        };

        var result = await ProjectTasksAppService.GetListAsync(input);
        KanbanItems = result.Items.Select(MapToKanbanItem).ToList();
        IsKanbanLoadedOnce = true;
        KanbanRenderKey++;
        await InvokeAsync(StateHasChanged);
    }

    private KanbanItem MapToKanbanItem(ProjectTaskWithNavigationPropertiesDto dto)
    {
        Enum.TryParse<ProjectTaskStatus>(dto.ProjectTask.Status, ignoreCase: true, out var status);
        Enum.TryParse<ProjectTaskPriority>(dto.ProjectTask.Priority, ignoreCase: true, out var priority);

        return new KanbanItem
        {
            Id = dto.ProjectTask.Id,
            ProjectName = dto.Project?.Name ?? string.Empty,
            Code = dto.ProjectTask.Code,
            Title = dto.ProjectTask.Title,
            Description = dto.ProjectTask.Description,
            DueDate = dto.ProjectTask.DueDate,
            Status = status,
            Priority = priority,
            ProgressPercent = dto.ProjectTask.ProgressPercent,
            Assignees = dto.ProjectTaskAssignments?
                .Select(x => x.User)
                .Where(u => u != null)
                .DistinctBy(u => u.Id)
                .ToList() ?? new List<IdentityUserDto>(),
            DocumentsCount = dto.ProjectTaskDocumentsCount,
            ProjectTask = dto.ProjectTask,
            ProjectTaskWithNavigationProperties = dto
        };
    }


    private async Task SwitchToKanbanAsync()
    {
        IsKanbanView = true;
        await RefreshKanbanAsync();
    }

    private Task SwitchToListAsync()
    {
        IsKanbanView = false;
        return Task.CompletedTask;
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

        // If user lands on List view first, ensure Kanban is loaded at least once
        // so counts/cards are ready when switching to Kanban.
        if (!IsKanbanLoadedOnce)
        {
            await RefreshKanbanAsync();
        }
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetProjectTasksAsync();
        await RefreshKanbanAsync();
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
        NewProjectTask = new ProjectTaskDto
        {
            StartDate = DateTime.Now,
            DueDate = DateTime.Now,
            Priority = ProjectTaskPriority.LOW.ToString(),
            Status = ProjectTaskStatus.TODO.ToString(),
        };

        // Defaults for enum-backed selects.
        NewProjectTaskPriority = ProjectTaskPriority.LOW;
        NewProjectTaskStatus = ProjectTaskStatus.TODO;

        SelectedNewProjectTaskProject = new List<LookupDto<Guid>>();
        SelectedNewProjectTaskParentTask = new List<ParentTaskSelectItem>();

        CreateWizardProjectTaskId = Guid.Empty;
        CreateGeneralValidationErrorKey = null;
        CreateAssignmentsUsersToAdd = new List<LookupDto<Guid>>();
        CreateAssignmentsList = new List<ProjectTaskAssignmentWithNavigationPropertiesDto>();
        CreateAssignmentRole = ProjectTaskAssignmentRole.MAIN;
        CreateAssignmentNote = null;
        CreateDocumentsToAdd = new List<LookupDto<Guid>>();
        CreateDocumentsList = new List<ProjectTaskDocumentWithNavigationPropertiesDto>();
        CreateDocumentPurpose = ProjectTaskDocumentPurpose.REPORT;
        SelectedCreateTab = "general";

        await GetProjectCollectionLookupAsync();
        await NewProjectTaskValidations.ClearAll();
        await CreateProjectTaskModal.Show();
    }

    private async Task CloseCreateProjectTaskModalAsync()
    {
        NewProjectTask = new ProjectTaskDto
        {
            StartDate = DateTime.Now,
            DueDate = DateTime.Now,
            Priority = ProjectTaskPriority.LOW.ToString(),
            Status = ProjectTaskStatus.TODO.ToString(),
        };
        CreateWizardProjectTaskId = Guid.Empty;
        CreateGeneralValidationErrorKey = null;
        CreateAssignmentsUsersToAdd = new List<LookupDto<Guid>>();
        CreateAssignmentsList = new List<ProjectTaskAssignmentWithNavigationPropertiesDto>();
        CreateDocumentsToAdd = new List<LookupDto<Guid>>();
        CreateDocumentsList = new List<ProjectTaskDocumentWithNavigationPropertiesDto>();
        SelectedCreateTab = "general";
        await CreateProjectTaskModal.Hide();
    }

    private async Task CancelCreateWizardAsync()
    {
        try
        {
            if (CreateWizardProjectTaskId != Guid.Empty)
            {
                if (!await UiMessageService.Confirm(L["CreateWizard:CancelAndDeleteTask"].Value))
                {
                    return;
                }

                // Best-effort cleanup to avoid leaving a task without assignments.
                await ProjectTasksAppService.DeleteAsync(CreateWizardProjectTaskId);
            }

            await CloseCreateProjectTaskModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task SaveGeneralInformationAsync()
    {
        try
        {
            if (IsCreateWizardGeneralSaved)
            {
                return;
            }

            if (!ValidateCreateGeneralInformation())
            {
                return;
            }

            var input = new ProjectTaskCreateDto
            {
                ParentTaskId = NewProjectTask.ParentTaskId,
                Code = NewProjectTask.Code,
                Title = NewProjectTask.Title,
                Description = NewProjectTask.Description,
                StartDate = NewProjectTask.StartDate,
                DueDate = NewProjectTask.DueDate,
                Priority = NewProjectTaskPriority.ToString(),
                Status = NewProjectTaskStatus.ToString(),
                ProgressPercent = NewProjectTask.ProgressPercent,
                ProjectId = NewProjectTask.ProjectId
            };

            var created = await ProjectTasksAppService.CreateAsync(input);
            CreateWizardProjectTaskId = created.Id;

            // Load step-2 data after task is created.
            await LoadCreateAssignmentsAsync();
            await LoadCreateDocumentsAsync();

            SelectedCreateTab = "assignments";
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private bool ValidateCreateGeneralInformation()
    {
        // Reset error state.
        CreateGeneralValidationErrorKey = null;

        // Required: Project
        if (NewProjectTask.ProjectId == Guid.Empty)
        {
            CreateGeneralValidationErrorKey = "ProjectRequired";
            InvokeAsync(StateHasChanged);
            return false;
        }

        // Required: Code
        if (string.IsNullOrWhiteSpace(NewProjectTask.Code))
        {
            CreateGeneralValidationErrorKey = "CodeRequired";
            InvokeAsync(StateHasChanged);
            return false;
        }

        // Required: Title
        if (string.IsNullOrWhiteSpace(NewProjectTask.Title))
        {
            CreateGeneralValidationErrorKey = "TitleRequired";
            InvokeAsync(StateHasChanged);
            return false;
        }

        // Required: Priority/Status are strings on DTO (enum-backed selects fill them).
        if (string.IsNullOrWhiteSpace(NewProjectTask.Priority))
        {
            CreateGeneralValidationErrorKey = "PriorityRequired";
            InvokeAsync(StateHasChanged);
            return false;
        }

        if (string.IsNullOrWhiteSpace(NewProjectTask.Status))
        {
            CreateGeneralValidationErrorKey = "StatusRequired";
            InvokeAsync(StateHasChanged);
            return false;
        }

        // Range: ProgressPercent
        if (NewProjectTask.ProgressPercent < ProjectTaskConsts.ProgressPercentMinLength
            || NewProjectTask.ProgressPercent > ProjectTaskConsts.ProgressPercentMaxLength)
        {
            CreateGeneralValidationErrorKey = "ProgressPercentRange";
            InvokeAsync(StateHasChanged);
            return false;
        }

        return true;
    }

    private async Task FinishCreateWizardAsync()
    {
        try
        {
            if (!IsCreateWizardGeneralSaved)
            {
                return;
            }

            if (CreateAssignmentsList.Count < 1)
            {
                await UiMessageService.Error(L["CreateWizard:AtLeastOneAssigneeRequired"]);
                SelectedCreateTab = "assignments";
                return;
            }

            await GetProjectTasksAsync();
            await RefreshKanbanAsync();
            await CloseCreateProjectTaskModalAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task LoadCreateAssignmentsAsync()
    {
        if (CreateWizardProjectTaskId == Guid.Empty)
        {
            CreateAssignmentsList = new List<ProjectTaskAssignmentWithNavigationPropertiesDto>();
            return;
        }

        var result = await ProjectTaskAssignmentsAppService.GetListAsync(new GetProjectTaskAssignmentsInput
        {
            ProjectTaskId = CreateWizardProjectTaskId,
            MaxResultCount = 1000,
            SkipCount = 0
        });

        CreateAssignmentsList = result.Items;
    }

    private async Task LoadCreateDocumentsAsync()
    {
        if (CreateWizardProjectTaskId == Guid.Empty)
        {
            CreateDocumentsList = new List<ProjectTaskDocumentWithNavigationPropertiesDto>();
            return;
        }

        var result = await ProjectTaskDocumentsAppService.GetListAsync(new GetProjectTaskDocumentsInput
        {
            ProjectTaskId = CreateWizardProjectTaskId,
            MaxResultCount = 1000,
            SkipCount = 0
        });

        CreateDocumentsList = result.Items;
    }

    protected async Task<List<LookupDto<Guid>>> GetAssignmentIdentityUserLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        var result = await ProjectTaskAssignmentsAppService.GetIdentityUserLookupAsync(new LookupRequestDto
        {
            Filter = filter,
            MaxResultCount = 20,
            SkipCount = 0
        });

        AssignmentIdentityUsersCollection = result.Items;
        return result.Items.ToList();
    }

    protected void OnCreateAssignmentUserChanged()
    {
        // Select2 (single-select) uses list; force rerender so Add button enables.
        InvokeAsync(StateHasChanged);
    }

    private async Task AddAssignmentAsync()
    {
        try
        {
            if (!IsCreateWizardGeneralSaved)
            {
                await UiMessageService.Error(L["CreateWizard:SaveGeneralFirst"]);
                return;
            }

            var userId = CreateAssignmentsUsersToAdd.FirstOrDefault()?.Id ?? Guid.Empty;
            if (userId == Guid.Empty)
            {
                await UiMessageService.Error(L["CreateWizard:AssigneeRequired"]);
                return;
            }

            await ProjectTaskAssignmentsAppService.CreateAsync(new ProjectTaskAssignmentCreateDto
            {
                ProjectTaskId = CreateWizardProjectTaskId,
                UserId = userId,
                AssignmentRole = CreateAssignmentRole.ToString(),
                AssignedAt = DateTime.Now,
                Note = CreateAssignmentNote
            });

            CreateAssignmentsUsersToAdd = new List<LookupDto<Guid>>();
            CreateAssignmentNote = null;
            await LoadCreateAssignmentsAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task DeleteAssignmentAsync(ProjectTaskAssignmentWithNavigationPropertiesDto row)
    {
        try
        {
            await ProjectTaskAssignmentsAppService.DeleteAsync(row.ProjectTaskAssignment.Id);
            await LoadCreateAssignmentsAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected async Task<List<LookupDto<Guid>>> GetDocumentLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        var result = await ProjectTaskDocumentsAppService.GetDocumentLookupAsync(new LookupRequestDto
        {
            Filter = filter,
            MaxResultCount = 20,
            SkipCount = 0
        });

        DocumentsLookupCollection = result.Items;
        return result.Items.ToList();
    }

    protected void OnCreateDocumentChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task AddDocumentAsync()
    {
        try
        {
            if (!IsCreateWizardGeneralSaved)
            {
                return;
            }

            var documentId = CreateDocumentsToAdd.FirstOrDefault()?.Id ?? Guid.Empty;
            if (documentId == Guid.Empty)
            {
                return;
            }

            await ProjectTaskDocumentsAppService.CreateAsync(new ProjectTaskDocumentCreateDto
            {
                ProjectTaskId = CreateWizardProjectTaskId,
                DocumentId = documentId,
                DocumentPurpose = CreateDocumentPurpose.ToString()
            });

            CreateDocumentsToAdd = new List<LookupDto<Guid>>();
            await LoadCreateDocumentsAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task DeleteDocumentAsync(ProjectTaskDocumentWithNavigationPropertiesDto row)
    {
        try
        {
            await ProjectTaskDocumentsAppService.DeleteAsync(row.ProjectTaskDocument.Id);
            await LoadCreateDocumentsAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
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
            await RefreshKanbanAsync();
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

            var input = new ProjectTaskCreateDto
            {
                ParentTaskId = NewProjectTask.ParentTaskId,
                Code = NewProjectTask.Code,
                Title = NewProjectTask.Title,
                Description = NewProjectTask.Description,
                StartDate = NewProjectTask.StartDate,
                DueDate = NewProjectTask.DueDate,
                Priority = NewProjectTaskPriority.ToString(),
                Status = NewProjectTaskStatus.ToString(),
                ProgressPercent = NewProjectTask.ProgressPercent,
                ProjectId = NewProjectTask.ProjectId
            };

            var created = await ProjectTasksAppService.CreateAsync(input);
            await GetProjectTasksAsync();
            await RefreshKanbanAsync();
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
            await RefreshKanbanAsync();
            await EditProjectTaskModal.Hide();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void OnSelectedCreateTabChanged(string name)
    {
        if ((name == "assignments" || name == "documents") && !IsCreateWizardGeneralSaved)
        {
            SelectedCreateTab = "general";
            return;
        }

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

    private async Task<List<LookupDto<Guid>>> GetProjectCollectionLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        ProjectsCollection = (await ProjectTasksAppService.GetProjectLookupAsync(new LookupRequestDto { Filter = filter })).Items;
        return ProjectsCollection.ToList();
    }

    private async Task<List<ParentTaskSelectItem>> GetParentTaskCollectionLookupAsync(IReadOnlyList<ParentTaskSelectItem> dbset, string filter, CancellationToken token)
    {
        var input = new GetProjectTasksInput
        {
            FilterText = filter,
            MaxResultCount = 20,
            SkipCount = 0,
        };

        // UI-first: use Code as parent id (string) because DTO uses ParentTaskId as string.
        var result = await ProjectTasksAppService.GetListAsync(input);
        ParentTasksCollection = result.Items
            .Select(x => new ParentTaskSelectItem
            {
                Id = x.ProjectTask.Code,
                DisplayName = $"{x.ProjectTask.Code} - {x.ProjectTask.Title}",
            })
            .ToList();

        return ParentTasksCollection.ToList();
    }

    protected void OnNewProjectTaskProjectChanged()
    {
        NewProjectTask.ProjectId = SelectedNewProjectTaskProject.FirstOrDefault()?.Id ?? Guid.Empty;
    }

    protected void OnNewProjectTaskParentChanged()
    {
        NewProjectTask.ParentTaskId = SelectedNewProjectTaskParentTask.FirstOrDefault()?.Id;
    }

    protected void OnNewProjectTaskPriorityChanged(ProjectTaskPriority priority)
    {
        NewProjectTaskPriority = priority;
        NewProjectTask.Priority = priority.ToString();
    }

    protected void OnNewProjectTaskStatusChanged(ProjectTaskStatus status)
    {
        NewProjectTaskStatus = status;
        NewProjectTask.Status = status.ToString();
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