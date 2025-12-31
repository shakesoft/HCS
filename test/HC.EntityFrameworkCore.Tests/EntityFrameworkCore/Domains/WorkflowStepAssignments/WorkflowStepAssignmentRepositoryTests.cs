using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.WorkflowStepAssignments;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.WorkflowStepAssignments;

public class WorkflowStepAssignmentRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IWorkflowStepAssignmentRepository _workflowStepAssignmentRepository;

    public WorkflowStepAssignmentRepositoryTests()
    {
        _workflowStepAssignmentRepository = GetRequiredService<IWorkflowStepAssignmentRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _workflowStepAssignmentRepository.GetListAsync(isPrimary: true, isActive: true);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("9b736dd1-b2d3-4e41-91cc-6d1f33f83ac1"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _workflowStepAssignmentRepository.GetCountAsync(isPrimary: true, isActive: true);
            // Assert
            result.ShouldBe(1);
        });
    }
}