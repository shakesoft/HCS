using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.WorkflowStepTemplates;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.WorkflowStepTemplates;

public class WorkflowStepTemplateRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IWorkflowStepTemplateRepository _workflowStepTemplateRepository;

    public WorkflowStepTemplateRepositoryTests()
    {
        _workflowStepTemplateRepository = GetRequiredService<IWorkflowStepTemplateRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _workflowStepTemplateRepository.GetListAsync(name: "1eb2615fe", type: "9b7d6a19632e483daff1", isActive: true);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("18f531f8-a877-4be1-97a8-dad9b1e53f0a"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _workflowStepTemplateRepository.GetCountAsync(name: "ea3139a2fb024e5997c70b99e1f4126898f34c874", type: "97b04b0b8f544d53a693", isActive: true);
            // Assert
            result.ShouldBe(1);
        });
    }
}