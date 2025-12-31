using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.WorkflowStepAssignments;

public abstract class WorkflowStepAssignmentsAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IWorkflowStepAssignmentsAppService _workflowStepAssignmentsAppService;
    private readonly IRepository<WorkflowStepAssignment, Guid> _workflowStepAssignmentRepository;

    public WorkflowStepAssignmentsAppServiceTests()
    {
        _workflowStepAssignmentsAppService = GetRequiredService<IWorkflowStepAssignmentsAppService>();
        _workflowStepAssignmentRepository = GetRequiredService<IRepository<WorkflowStepAssignment, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _workflowStepAssignmentsAppService.GetListAsync(new GetWorkflowStepAssignmentsInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.WorkflowStepAssignment.Id == Guid.Parse("9b736dd1-b2d3-4e41-91cc-6d1f33f83ac1")).ShouldBe(true);
        result.Items.Any(x => x.WorkflowStepAssignment.Id == Guid.Parse("178db716-2dcb-4306-a6b3-768f37eea54f")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _workflowStepAssignmentsAppService.GetAsync(Guid.Parse("9b736dd1-b2d3-4e41-91cc-6d1f33f83ac1"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("9b736dd1-b2d3-4e41-91cc-6d1f33f83ac1"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new WorkflowStepAssignmentCreateDto
        {
            IsPrimary = true,
            IsActive = true
        };
        // Act
        var serviceResult = await _workflowStepAssignmentsAppService.CreateAsync(input);
        // Assert
        var result = await _workflowStepAssignmentRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.IsPrimary.ShouldBe(true);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new WorkflowStepAssignmentUpdateDto()
        {
            IsPrimary = true,
            IsActive = true
        };
        // Act
        var serviceResult = await _workflowStepAssignmentsAppService.UpdateAsync(Guid.Parse("9b736dd1-b2d3-4e41-91cc-6d1f33f83ac1"), input);
        // Assert
        var result = await _workflowStepAssignmentRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.IsPrimary.ShouldBe(true);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _workflowStepAssignmentsAppService.DeleteAsync(Guid.Parse("9b736dd1-b2d3-4e41-91cc-6d1f33f83ac1"));
        // Assert
        var result = await _workflowStepAssignmentRepository.FindAsync(c => c.Id == Guid.Parse("9b736dd1-b2d3-4e41-91cc-6d1f33f83ac1"));
        result.ShouldBeNull();
    }
}