using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.WorkflowDefinitions;

public abstract class WorkflowDefinitionsAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IWorkflowDefinitionsAppService _workflowDefinitionsAppService;
    private readonly IRepository<WorkflowDefinition, Guid> _workflowDefinitionRepository;

    public WorkflowDefinitionsAppServiceTests()
    {
        _workflowDefinitionsAppService = GetRequiredService<IWorkflowDefinitionsAppService>();
        _workflowDefinitionRepository = GetRequiredService<IRepository<WorkflowDefinition, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _workflowDefinitionsAppService.GetListAsync(new GetWorkflowDefinitionsInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.Id == Guid.Parse("0e604ee9-61cf-473d-baf9-a02f14e32d58")).ShouldBe(true);
        result.Items.Any(x => x.Id == Guid.Parse("5707f147-c6d0-4094-b280-16c7db497a05")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _workflowDefinitionsAppService.GetAsync(Guid.Parse("0e604ee9-61cf-473d-baf9-a02f14e32d58"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("0e604ee9-61cf-473d-baf9-a02f14e32d58"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new WorkflowDefinitionCreateDto
        {
            Code = "257304fb39044a66a57fffaccb5372540187f11f5eb341009a",
            Name = "090ddce15797459f970a762204737619693624bcfd9745f6b82c641f59fa314b86416e0fcc9645809a9cf3",
            Description = "268ff05bc4cc44029fe2621b51859813949324afc98f47748b857e132380d80",
            IsActive = true
        };
        // Act
        var serviceResult = await _workflowDefinitionsAppService.CreateAsync(input);
        // Assert
        var result = await _workflowDefinitionRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("257304fb39044a66a57fffaccb5372540187f11f5eb341009a");
        result.Name.ShouldBe("090ddce15797459f970a762204737619693624bcfd9745f6b82c641f59fa314b86416e0fcc9645809a9cf3");
        result.Description.ShouldBe("268ff05bc4cc44029fe2621b51859813949324afc98f47748b857e132380d80");
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new WorkflowDefinitionUpdateDto()
        {
            Code = "44ed2baeb75647e6b3b067866f299dd897fd9bd7ecf14715b2",
            Name = "892aea00759e48e1ae9024bc618a7b1595932aad2bf342b29e759526982cd342d20a52725d73427",
            Description = "7b9426eba47f4479bcf35aaa453dc8c5",
            IsActive = true
        };
        // Act
        var serviceResult = await _workflowDefinitionsAppService.UpdateAsync(Guid.Parse("0e604ee9-61cf-473d-baf9-a02f14e32d58"), input);
        // Assert
        var result = await _workflowDefinitionRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("44ed2baeb75647e6b3b067866f299dd897fd9bd7ecf14715b2");
        result.Name.ShouldBe("892aea00759e48e1ae9024bc618a7b1595932aad2bf342b29e759526982cd342d20a52725d73427");
        result.Description.ShouldBe("7b9426eba47f4479bcf35aaa453dc8c5");
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _workflowDefinitionsAppService.DeleteAsync(Guid.Parse("0e604ee9-61cf-473d-baf9-a02f14e32d58"));
        // Assert
        var result = await _workflowDefinitionRepository.FindAsync(c => c.Id == Guid.Parse("0e604ee9-61cf-473d-baf9-a02f14e32d58"));
        result.ShouldBeNull();
    }
}