using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HC.ProjectTaskAssignments;
using HC.ProjectTaskDocuments;
using HC.ProjectTasks;
using HC.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace HC.Blazor.Pages;

public partial class ProjectTasks
{
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

    private async Task LoadEditAssignmentsAsync()
    {
        if (EditingProjectTaskId == Guid.Empty)
        {
            EditAssignmentsList = new List<ProjectTaskAssignmentWithNavigationPropertiesDto>();
            return;
        }

        var result = await ProjectTaskAssignmentsAppService.GetListAsync(new GetProjectTaskAssignmentsInput
        {
            ProjectTaskId = EditingProjectTaskId,
            MaxResultCount = 1000,
            SkipCount = 0
        });

        EditAssignmentsList = result.Items;
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

    private async Task LoadEditDocumentsAsync()
    {
        if (EditingProjectTaskId == Guid.Empty)
        {
            EditDocumentsList = new List<ProjectTaskDocumentWithNavigationPropertiesDto>();
            return;
        }

        var result = await ProjectTaskDocumentsAppService.GetListAsync(new GetProjectTaskDocumentsInput
        {
            ProjectTaskId = EditingProjectTaskId,
            MaxResultCount = 1000,
            SkipCount = 0
        });

        EditDocumentsList = result.Items;
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

    protected void OnEditAssignmentUserChanged()
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

    private async Task AddEditAssignmentAsync()
    {
        try
        {
            if (EditingProjectTaskId == Guid.Empty)
            {
                return;
            }

            var userId = EditAssignmentsUsersToAdd.FirstOrDefault()?.Id ?? Guid.Empty;
            if (userId == Guid.Empty)
            {
                await UiMessageService.Error(L["CreateWizard:AssigneeRequired"]);
                return;
            }

            await ProjectTaskAssignmentsAppService.CreateAsync(new ProjectTaskAssignmentCreateDto
            {
                ProjectTaskId = EditingProjectTaskId,
                UserId = userId,
                AssignmentRole = EditAssignmentRole.ToString(),
                AssignedAt = DateTime.Now,
                Note = EditAssignmentNote
            });

            EditAssignmentsUsersToAdd = new List<LookupDto<Guid>>();
            EditAssignmentNote = null;
            await LoadEditAssignmentsAsync();
            await RefreshKanbanAsync();
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

    private async Task DeleteEditAssignmentAsync(ProjectTaskAssignmentWithNavigationPropertiesDto row)
    {
        try
        {
            await ProjectTaskAssignmentsAppService.DeleteAsync(row.ProjectTaskAssignment.Id);
            await LoadEditAssignmentsAsync();
            await RefreshKanbanAsync();
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

    protected void OnEditDocumentChanged()
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

    private async Task AddEditDocumentAsync()
    {
        try
        {
            if (EditingProjectTaskId == Guid.Empty)
            {
                return;
            }

            var documentId = EditDocumentsToAdd.FirstOrDefault()?.Id ?? Guid.Empty;
            if (documentId == Guid.Empty)
            {
                return;
            }

            await ProjectTaskDocumentsAppService.CreateAsync(new ProjectTaskDocumentCreateDto
            {
                ProjectTaskId = EditingProjectTaskId,
                DocumentId = documentId,
                DocumentPurpose = EditDocumentPurpose.ToString()
            });

            EditDocumentsToAdd = new List<LookupDto<Guid>>();
            await LoadEditDocumentsAsync();
            await RefreshKanbanAsync();
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

    private async Task DeleteEditDocumentAsync(ProjectTaskDocumentWithNavigationPropertiesDto row)
    {
        try
        {
            await ProjectTaskDocumentsAppService.DeleteAsync(row.ProjectTaskDocument.Id);
            await LoadEditDocumentsAsync();
            await RefreshKanbanAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OpenEditProjectTaskModalAsync(ProjectTaskWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "general";
        var projectTask = await ProjectTasksAppService.GetWithNavigationPropertiesAsync(input.ProjectTask.Id);
        EditingProjectTaskId = projectTask.ProjectTask.Id;
        EditingProjectTask = ObjectMapper.Map<ProjectTaskDto, ProjectTaskUpdateDto>(projectTask.ProjectTask);
        EditGeneralValidationErrorKey = null;

        // Initialize Select2 selections for Project and ParentTask.
        SelectedEditProjectTaskProject = new List<LookupDto<Guid>>();
        if (EditingProjectTask.ProjectId != Guid.Empty)
        {
            var displayName = projectTask.Project?.Name ?? string.Empty;
            var selectedProject = new LookupDto<Guid>
            {
                Id = EditingProjectTask.ProjectId,
                DisplayName = string.IsNullOrWhiteSpace(displayName) ? EditingProjectTask.ProjectId.ToString() : displayName
            };

            SelectedEditProjectTaskProject.Add(selectedProject);
            ProjectsCollection = ProjectsCollection.Concat(new[] { selectedProject }).DistinctBy(x => x.Id).ToList();
        }

        SelectedEditProjectTaskParentTask = new List<ParentTaskSelectItem>();
        if (!string.IsNullOrWhiteSpace(EditingProjectTask.ParentTaskId))
        {
            var parent = new ParentTaskSelectItem
            {
                Id = EditingProjectTask.ParentTaskId!,
                DisplayName = EditingProjectTask.ParentTaskId!
            };
            SelectedEditProjectTaskParentTask.Add(parent);
            ParentTasksCollection = ParentTasksCollection.Concat(new[] { parent }).DistinctBy(x => x.Id).ToList();
        }

        // Initialize enum selects from DTO strings.
        if (Enum.TryParse<ProjectTaskPriority>(EditingProjectTask.Priority, ignoreCase: true, out var priority))
        {
            EditingProjectTaskPriority = priority;
        }
        if (Enum.TryParse<ProjectTaskStatus>(EditingProjectTask.Status, ignoreCase: true, out var status))
        {
            EditingProjectTaskStatus = status;
        }

        // Load edit tabs data.
        EditAssignmentsUsersToAdd = new List<LookupDto<Guid>>();
        EditAssignmentRole = ProjectTaskAssignmentRole.MAIN;
        EditAssignmentNote = null;
        EditDocumentsToAdd = new List<LookupDto<Guid>>();
        EditDocumentPurpose = ProjectTaskDocumentPurpose.REPORT;
        await LoadEditAssignmentsAsync();
        await LoadEditDocumentsAsync();

        await EditProjectTaskModal.Show();
    }

    protected void OnEditProjectTaskProjectChanged()
    {
        EditingProjectTask.ProjectId = SelectedEditProjectTaskProject.FirstOrDefault()?.Id ?? Guid.Empty;
    }

    protected void OnEditProjectTaskParentChanged()
    {
        EditingProjectTask.ParentTaskId = SelectedEditProjectTaskParentTask.FirstOrDefault()?.Id;
    }

    protected void OnEditingProjectTaskPriorityChanged(ProjectTaskPriority priority)
    {
        EditingProjectTaskPriority = priority;
        EditingProjectTask.Priority = priority.ToString();
    }

    protected void OnEditingProjectTaskStatusChanged(ProjectTaskStatus status)
    {
        EditingProjectTaskStatus = status;
        EditingProjectTask.Status = status.ToString();
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

            await ProjectTasksAppService.CreateAsync(input);
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
            if (!ValidateEditGeneralInformation())
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

    private bool ValidateEditGeneralInformation()
    {
        // Reset error state.
        EditGeneralValidationErrorKey = null;

        // Required: Project
        if (EditingProjectTask.ProjectId == Guid.Empty)
        {
            EditGeneralValidationErrorKey = "ProjectRequired";
            InvokeAsync(StateHasChanged);
            return false;
        }

        // Required: Code
        if (string.IsNullOrWhiteSpace(EditingProjectTask.Code))
        {
            EditGeneralValidationErrorKey = "CodeRequired";
            InvokeAsync(StateHasChanged);
            return false;
        }

        // Required: Title
        if (string.IsNullOrWhiteSpace(EditingProjectTask.Title))
        {
            EditGeneralValidationErrorKey = "TitleRequired";
            InvokeAsync(StateHasChanged);
            return false;
        }

        // Required: Priority/Status are strings on DTO (enum-backed selects fill them).
        if (string.IsNullOrWhiteSpace(EditingProjectTask.Priority))
        {
            EditGeneralValidationErrorKey = "PriorityRequired";
            InvokeAsync(StateHasChanged);
            return false;
        }

        if (string.IsNullOrWhiteSpace(EditingProjectTask.Status))
        {
            EditGeneralValidationErrorKey = "StatusRequired";
            InvokeAsync(StateHasChanged);
            return false;
        }

        // Range: ProgressPercent
        if (EditingProjectTask.ProgressPercent < ProjectTaskConsts.ProgressPercentMinLength
            || EditingProjectTask.ProgressPercent > ProjectTaskConsts.ProgressPercentMaxLength)
        {
            EditGeneralValidationErrorKey = "ProgressPercentRange";
            InvokeAsync(StateHasChanged);
            return false;
        }

        return true;
    }
}