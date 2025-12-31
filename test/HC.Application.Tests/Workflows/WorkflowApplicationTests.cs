using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.Workflows;

public abstract class WorkflowsAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IWorkflowsAppService _workflowsAppService;
    private readonly IRepository<Workflow, Guid> _workflowRepository;

    public WorkflowsAppServiceTests()
    {
        _workflowsAppService = GetRequiredService<IWorkflowsAppService>();
        _workflowRepository = GetRequiredService<IRepository<Workflow, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _workflowsAppService.GetListAsync(new GetWorkflowsInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.Id == Guid.Parse("66523a7a-3dcb-4880-a5c2-ae55ac3c4656")).ShouldBe(true);
        result.Items.Any(x => x.Id == Guid.Parse("0e294610-d894-4dd6-bc3a-823341ce02fb")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _workflowsAppService.GetAsync(Guid.Parse("66523a7a-3dcb-4880-a5c2-ae55ac3c4656"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("66523a7a-3dcb-4880-a5c2-ae55ac3c4656"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new WorkflowCreateDto
        {
            Code = "9802d705cb514a43827e13df6b196fd44880b21cf9ac4fc686",
            Name = "4afb2bcaedb14cb3bb271ba45eec8693454230ae41864aa89e4c714e2b02928d10db6effc7594578810f4f6a7fd0250",
            Description = "5b1c9a88015f47a4b3f",
            IsActive = true
        };
        // Act
        var serviceResult = await _workflowsAppService.CreateAsync(input);
        // Assert
        var result = await _workflowRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("9802d705cb514a43827e13df6b196fd44880b21cf9ac4fc686");
        result.Name.ShouldBe("4afb2bcaedb14cb3bb271ba45eec8693454230ae41864aa89e4c714e2b02928d10db6effc7594578810f4f6a7fd0250");
        result.Description.ShouldBe("5b1c9a88015f47a4b3f");
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new WorkflowUpdateDto()
        {
            Code = "03418d02c2b94876a78fa6a03fd368e461e5c84306884b0aba",
            Name = "66b7f63637604b6d8f44cfbffc37c8dd2386515be0044d7",
            Description = "5582e46e08714eadba59399169b2a3227f37c288557e48718d6d285d7cbe2404e3c5b7bd2ae",
            IsActive = true
        };
        // Act
        var serviceResult = await _workflowsAppService.UpdateAsync(Guid.Parse("66523a7a-3dcb-4880-a5c2-ae55ac3c4656"), input);
        // Assert
        var result = await _workflowRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("03418d02c2b94876a78fa6a03fd368e461e5c84306884b0aba");
        result.Name.ShouldBe("66b7f63637604b6d8f44cfbffc37c8dd2386515be0044d7");
        result.Description.ShouldBe("5582e46e08714eadba59399169b2a3227f37c288557e48718d6d285d7cbe2404e3c5b7bd2ae");
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _workflowsAppService.DeleteAsync(Guid.Parse("66523a7a-3dcb-4880-a5c2-ae55ac3c4656"));
        // Assert
        var result = await _workflowRepository.FindAsync(c => c.Id == Guid.Parse("66523a7a-3dcb-4880-a5c2-ae55ac3c4656"));
        result.ShouldBeNull();
    }
}