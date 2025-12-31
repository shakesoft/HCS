using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.DocumentHistories;

public abstract class DocumentHistoriesAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IDocumentHistoriesAppService _documentHistoriesAppService;
    private readonly IRepository<DocumentHistory, Guid> _documentHistoryRepository;

    public DocumentHistoriesAppServiceTests()
    {
        _documentHistoriesAppService = GetRequiredService<IDocumentHistoriesAppService>();
        _documentHistoryRepository = GetRequiredService<IRepository<DocumentHistory, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _documentHistoriesAppService.GetListAsync(new GetDocumentHistoriesInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.DocumentHistory.Id == Guid.Parse("1a52ebe0-8da8-446e-9fa5-19c062b5fe6c")).ShouldBe(true);
        result.Items.Any(x => x.DocumentHistory.Id == Guid.Parse("53b386e1-5508-4466-a7b9-abee48fb47b3")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _documentHistoriesAppService.GetAsync(Guid.Parse("1a52ebe0-8da8-446e-9fa5-19c062b5fe6c"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("1a52ebe0-8da8-446e-9fa5-19c062b5fe6c"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new DocumentHistoryCreateDto
        {
            Comment = "67c7fce116874a3a807eaa9824760e825b9011f92689412f99abb34d8aedba2655647f49367e477e89e",
            Action = "eeaf14228ff44953b1804c296ddc34",
            DocumentId = ,
            ToUser =
        };
        // Act
        var serviceResult = await _documentHistoriesAppService.CreateAsync(input);
        // Assert
        var result = await _documentHistoryRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Comment.ShouldBe("67c7fce116874a3a807eaa9824760e825b9011f92689412f99abb34d8aedba2655647f49367e477e89e");
        result.Action.ShouldBe("eeaf14228ff44953b1804c296ddc34");
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new DocumentHistoryUpdateDto()
        {
            Comment = "5a52dbe2b25c4aad97b",
            Action = "0882b13839594a498f7250a2af1108",
            DocumentId = ,
            ToUser =
        };
        // Act
        var serviceResult = await _documentHistoriesAppService.UpdateAsync(Guid.Parse("1a52ebe0-8da8-446e-9fa5-19c062b5fe6c"), input);
        // Assert
        var result = await _documentHistoryRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Comment.ShouldBe("5a52dbe2b25c4aad97b");
        result.Action.ShouldBe("0882b13839594a498f7250a2af1108");
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _documentHistoriesAppService.DeleteAsync(Guid.Parse("1a52ebe0-8da8-446e-9fa5-19c062b5fe6c"));
        // Assert
        var result = await _documentHistoryRepository.FindAsync(c => c.Id == Guid.Parse("1a52ebe0-8da8-446e-9fa5-19c062b5fe6c"));
        result.ShouldBeNull();
    }
}