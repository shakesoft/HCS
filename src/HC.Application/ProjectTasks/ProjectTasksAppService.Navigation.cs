using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HC.ProjectTaskAssignments;
using Volo.Abp.Application.Dtos;

namespace HC.ProjectTasks;

public partial class ProjectTasksAppService
{
    public override async Task<PagedResultDto<ProjectTaskWithNavigationPropertiesDto>> GetListAsync(GetProjectTasksInput input)
    {
        var result = await base.GetListAsync(input);

        await EnrichNavigationForTasksAsync(result.Items);

        return result;
    }

    public override async Task<ProjectTaskWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
    {
        var dto = await base.GetWithNavigationPropertiesAsync(id);

        await EnrichNavigationForTasksAsync(new List<ProjectTaskWithNavigationPropertiesDto> { dto });

        return dto;
    }

    private async Task EnrichNavigationForTasksAsync(IReadOnlyList<ProjectTaskWithNavigationPropertiesDto> tasks)
    {
        // Best-effort enrichment for UI; base task list should still work even if there's no related data.
        var taskIds = tasks
            .Where(x => x.ProjectTask != null && x.ProjectTask.Id != Guid.Empty)
            .Select(x => x.ProjectTask.Id)
            .Distinct()
            .ToList();

        if (taskIds.Count == 0)
        {
            return;
        }

        var assignments = await _projectTaskAssignmentRepository.GetListWithNavigationPropertiesByProjectTaskIdsAsync(taskIds);
        var assignmentDtos = ObjectMapper.Map<List<ProjectTaskAssignmentWithNavigationProperties>, List<ProjectTaskAssignmentWithNavigationPropertiesDto>>(assignments);

        var assignmentsByTaskId = assignmentDtos
            .Where(x => x.ProjectTaskAssignment.ProjectTaskId != Guid.Empty)
            .GroupBy(x => x.ProjectTaskAssignment.ProjectTaskId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var documentCountByTaskId = await _projectTaskDocumentRepository.GetCountByProjectTaskIdsAsync(taskIds);

        // Parent/child enrichment (ParentTaskId stores parent task Code as string).
        var parentCodes = tasks
            .Select(x => x.ProjectTask.ParentTaskId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var taskCodes = tasks
            .Select(x => x.ProjectTask.Code)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var parentTitleByCode = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var childCountByParentCode = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var query = await _projectTaskRepository.GetQueryableAsync();

        if (parentCodes.Count > 0)
        {
            var parents = await AsyncExecuter.ToListAsync(query
                .Where(x => parentCodes.Contains(x.Code))
                .Select(x => new { x.Code, x.Title }));

            parentTitleByCode = parents
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .ToDictionary(x => x.Code, x => x.Title ?? string.Empty, StringComparer.OrdinalIgnoreCase);
        }

        if (taskCodes.Count > 0)
        {
            var childCounts = await AsyncExecuter.ToListAsync(query
                .Where(x => !string.IsNullOrWhiteSpace(x.ParentTaskId) && taskCodes.Contains(x.ParentTaskId!))
                .GroupBy(x => x.ParentTaskId!)
                .Select(g => new { ParentCode = g.Key, Count = g.Count() }));

            childCountByParentCode = childCounts
                .Where(x => !string.IsNullOrWhiteSpace(x.ParentCode))
                .ToDictionary(x => x.ParentCode, x => x.Count, StringComparer.OrdinalIgnoreCase);
        }

        foreach (var task in tasks)
        {
            var taskId = task.ProjectTask.Id;

            task.ProjectTaskAssignments = assignmentsByTaskId.TryGetValue(taskId, out var list)
                ? list
                : new List<ProjectTaskAssignmentWithNavigationPropertiesDto>();

            task.ProjectTaskDocumentsCount = documentCountByTaskId.TryGetValue(taskId, out var count)
                ? count
                : 0;

            // Child tasks: show parent label on title.
            if (!string.IsNullOrWhiteSpace(task.ProjectTask.ParentTaskId)
                && parentTitleByCode.TryGetValue(task.ProjectTask.ParentTaskId, out var parentTitle))
            {
                task.ParentTaskTitle = parentTitle;
            }
            else
            {
                task.ParentTaskTitle = null;
            }

            // Parent tasks: show number of child tasks.
            task.ChildTaskCount = childCountByParentCode.TryGetValue(task.ProjectTask.Code, out var childCount)
                ? childCount
                : 0;
        }
    }
}

