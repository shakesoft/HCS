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

        foreach (var task in tasks)
        {
            var taskId = task.ProjectTask.Id;

            task.ProjectTaskAssignments = assignmentsByTaskId.TryGetValue(taskId, out var list)
                ? list
                : new List<ProjectTaskAssignmentWithNavigationPropertiesDto>();

            task.ProjectTaskDocumentsCount = documentCountByTaskId.TryGetValue(taskId, out var count)
                ? count
                : 0;
        }
    }
}

