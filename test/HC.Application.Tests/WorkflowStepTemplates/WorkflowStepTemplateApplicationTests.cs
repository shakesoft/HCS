using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplatesAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IWorkflowStepTemplatesAppService _workflowStepTemplatesAppService;
    private readonly IRepository<WorkflowStepTemplate, Guid> _workflowStepTemplateRepository;

    public WorkflowStepTemplatesAppServiceTests()
    {
        _workflowStepTemplatesAppService = GetRequiredService<IWorkflowStepTemplatesAppService>();
        _workflowStepTemplateRepository = GetRequiredService<IRepository<WorkflowStepTemplate, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _workflowStepTemplatesAppService.GetListAsync(new GetWorkflowStepTemplatesInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.WorkflowStepTemplate.Id == Guid.Parse("18f531f8-a877-4be1-97a8-dad9b1e53f0a")).ShouldBe(true);
        result.Items.Any(x => x.WorkflowStepTemplate.Id == Guid.Parse("e003dd39-1e46-4cd8-9b4d-425be811abe0")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _workflowStepTemplatesAppService.GetAsync(Guid.Parse("18f531f8-a877-4be1-97a8-dad9b1e53f0a"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("18f531f8-a877-4be1-97a8-dad9b1e53f0a"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new WorkflowStepTemplateCreateDto
        {
            Order = 6259,
            Name = "d94a55e8228f446b97cebacbde81",
            Type = "9be682655549449492db",
            SLADays = 1964886140,
            AllowReturn = true,
            IsActive = true,
            WorkflowId =
        };
        // Act
        var serviceResult = await _workflowStepTemplatesAppService.CreateAsync(input);
        // Assert
        var result = await _workflowStepTemplateRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Order.ShouldBe(6259);
        result.Name.ShouldBe("d94a55e8228f446b97cebacbde81");
        result.Type.ShouldBe("9be682655549449492db");
        result.SLADays.ShouldBe(1964886140);
        result.AllowReturn.ShouldBe(true);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new WorkflowStepTemplateUpdateDto()
        {
            Order = 6455,
            Name = "ea75acd3b030425ea7100fa845927c49a5b87878753e44458539fbf522cf81d37318334ad48e42dcbfd5f9f7c8d3f2",
            Type = "f4520403483040a68d86",
            SLADays = 871577850,
            AllowReturn = true,
            IsActive = true,
            WorkflowId =
        };
        // Act
        var serviceResult = await _workflowStepTemplatesAppService.UpdateAsync(Guid.Parse("18f531f8-a877-4be1-97a8-dad9b1e53f0a"), input);
        // Assert
        var result = await _workflowStepTemplateRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Order.ShouldBe(6455);
        result.Name.ShouldBe("ea75acd3b030425ea7100fa845927c49a5b87878753e44458539fbf522cf81d37318334ad48e42dcbfd5f9f7c8d3f2");
        result.Type.ShouldBe("f4520403483040a68d86");
        result.SLADays.ShouldBe(871577850);
        result.AllowReturn.ShouldBe(true);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _workflowStepTemplatesAppService.DeleteAsync(Guid.Parse("18f531f8-a877-4be1-97a8-dad9b1e53f0a"));
        // Assert
        var result = await _workflowStepTemplateRepository.FindAsync(c => c.Id == Guid.Parse("18f531f8-a877-4be1-97a8-dad9b1e53f0a"));
        result.ShouldBeNull();
    }
}