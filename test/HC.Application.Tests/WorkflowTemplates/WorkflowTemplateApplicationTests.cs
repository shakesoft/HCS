using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.WorkflowTemplates;

public abstract class WorkflowTemplatesAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IWorkflowTemplatesAppService _workflowTemplatesAppService;
    private readonly IRepository<WorkflowTemplate, Guid> _workflowTemplateRepository;

    public WorkflowTemplatesAppServiceTests()
    {
        _workflowTemplatesAppService = GetRequiredService<IWorkflowTemplatesAppService>();
        _workflowTemplateRepository = GetRequiredService<IRepository<WorkflowTemplate, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _workflowTemplatesAppService.GetListAsync(new GetWorkflowTemplatesInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.WorkflowTemplate.Id == Guid.Parse("42ef75ac-fa52-448b-baae-26c32dcc7c76")).ShouldBe(true);
        result.Items.Any(x => x.WorkflowTemplate.Id == Guid.Parse("428b2b94-8f09-41f2-aa9c-e8f75bd8c34a")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _workflowTemplatesAppService.GetAsync(Guid.Parse("42ef75ac-fa52-448b-baae-26c32dcc7c76"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("42ef75ac-fa52-448b-baae-26c32dcc7c76"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new WorkflowTemplateCreateDto
        {
            Code = "6b326fd5e9284352b4367cf09f1a7bd985f47ca5775c42dc9e",
            Name = "a23b253a0fae466b987f35a1ebda7736dadbd434ca6e436d811afec4da580b564af66a810de9461",
            WordTemplatePath = "608ac7735d9a46759ffa725bf43e9ef0376af5ad62874cbf8ffa4b0f213edd92ccb8727f95",
            ContentSchema = "bd33a75e6d5842c0b3a4775b9daf6e4ae3a25ba9ec1d409791fe2fee",
            OutputFormat = "8c90643e1023400ab2d9",
            SignMode = "70cd6cc8f46f46d9a3f9",
            WorkflowId =
        };
        // Act
        var serviceResult = await _workflowTemplatesAppService.CreateAsync(input);
        // Assert
        var result = await _workflowTemplateRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("6b326fd5e9284352b4367cf09f1a7bd985f47ca5775c42dc9e");
        result.Name.ShouldBe("a23b253a0fae466b987f35a1ebda7736dadbd434ca6e436d811afec4da580b564af66a810de9461");
        result.WordTemplatePath.ShouldBe("608ac7735d9a46759ffa725bf43e9ef0376af5ad62874cbf8ffa4b0f213edd92ccb8727f95");
        result.ContentSchema.ShouldBe("bd33a75e6d5842c0b3a4775b9daf6e4ae3a25ba9ec1d409791fe2fee");
        result.OutputFormat.ShouldBe("8c90643e1023400ab2d9");
        result.SignMode.ShouldBe("70cd6cc8f46f46d9a3f9");
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new WorkflowTemplateUpdateDto()
        {
            Code = "22e73e99c7e249da84d9ee9795c938b61484e7200d72487fbe",
            Name = "ba946a9940544561a785fc11251",
            WordTemplatePath = "0720d6289f534c0d82ed4d82a479b8c202527dacd42e4",
            ContentSchema = "ce42c5684c4a4810b1837b0fa55bb4f05f",
            OutputFormat = "c3c53b13ba154d368861",
            SignMode = "e1de5754184949159400",
            WorkflowId =
        };
        // Act
        var serviceResult = await _workflowTemplatesAppService.UpdateAsync(Guid.Parse("42ef75ac-fa52-448b-baae-26c32dcc7c76"), input);
        // Assert
        var result = await _workflowTemplateRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("22e73e99c7e249da84d9ee9795c938b61484e7200d72487fbe");
        result.Name.ShouldBe("ba946a9940544561a785fc11251");
        result.WordTemplatePath.ShouldBe("0720d6289f534c0d82ed4d82a479b8c202527dacd42e4");
        result.ContentSchema.ShouldBe("ce42c5684c4a4810b1837b0fa55bb4f05f");
        result.OutputFormat.ShouldBe("c3c53b13ba154d368861");
        result.SignMode.ShouldBe("e1de5754184949159400");
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _workflowTemplatesAppService.DeleteAsync(Guid.Parse("42ef75ac-fa52-448b-baae-26c32dcc7c76"));
        // Assert
        var result = await _workflowTemplateRepository.FindAsync(c => c.Id == Guid.Parse("42ef75ac-fa52-448b-baae-26c32dcc7c76"));
        result.ShouldBeNull();
    }
}