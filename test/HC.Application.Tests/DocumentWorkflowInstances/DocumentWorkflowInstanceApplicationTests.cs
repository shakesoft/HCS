using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.DocumentWorkflowInstances;

public abstract class DocumentWorkflowInstancesAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IDocumentWorkflowInstancesAppService _documentWorkflowInstancesAppService;
    private readonly IRepository<DocumentWorkflowInstance, Guid> _documentWorkflowInstanceRepository;

    public DocumentWorkflowInstancesAppServiceTests()
    {
        _documentWorkflowInstancesAppService = GetRequiredService<IDocumentWorkflowInstancesAppService>();
        _documentWorkflowInstanceRepository = GetRequiredService<IRepository<DocumentWorkflowInstance, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _documentWorkflowInstancesAppService.GetListAsync(new GetDocumentWorkflowInstancesInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.DocumentWorkflowInstance.Id == Guid.Parse("10bc60a8-1b25-4c98-be34-4237f3cbabe6")).ShouldBe(true);
        result.Items.Any(x => x.DocumentWorkflowInstance.Id == Guid.Parse("a175862c-8312-4f26-abe8-05ba7413bd3a")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _documentWorkflowInstancesAppService.GetAsync(Guid.Parse("10bc60a8-1b25-4c98-be34-4237f3cbabe6"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("10bc60a8-1b25-4c98-be34-4237f3cbabe6"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new DocumentWorkflowInstanceCreateDto
        {
            Status = "bc86052f3f8944abb66f",
            StartedAt = new DateTime(2004, 11, 14),
            FinishedAt = new DateTime(2008, 5, 15),
            DocumentId = ,
            WorkflowId = ,
            WorkflowTemplateId = ,
            CurrentStepId =
        };
        // Act
        var serviceResult = await _documentWorkflowInstancesAppService.CreateAsync(input);
        // Assert
        var result = await _documentWorkflowInstanceRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Status.ShouldBe("bc86052f3f8944abb66f");
        result.StartedAt.ShouldBe(new DateTime(2004, 11, 14));
        result.FinishedAt.ShouldBe(new DateTime(2008, 5, 15));
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new DocumentWorkflowInstanceUpdateDto()
        {
            Status = "efefdec7ea774b15acde",
            StartedAt = new DateTime(2017, 9, 8),
            FinishedAt = new DateTime(2007, 11, 18),
            DocumentId = ,
            WorkflowId = ,
            WorkflowTemplateId = ,
            CurrentStepId =
        };
        // Act
        var serviceResult = await _documentWorkflowInstancesAppService.UpdateAsync(Guid.Parse("10bc60a8-1b25-4c98-be34-4237f3cbabe6"), input);
        // Assert
        var result = await _documentWorkflowInstanceRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Status.ShouldBe("efefdec7ea774b15acde");
        result.StartedAt.ShouldBe(new DateTime(2017, 9, 8));
        result.FinishedAt.ShouldBe(new DateTime(2007, 11, 18));
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _documentWorkflowInstancesAppService.DeleteAsync(Guid.Parse("10bc60a8-1b25-4c98-be34-4237f3cbabe6"));
        // Assert
        var result = await _documentWorkflowInstanceRepository.FindAsync(c => c.Id == Guid.Parse("10bc60a8-1b25-4c98-be34-4237f3cbabe6"));
        result.ShouldBeNull();
    }
}