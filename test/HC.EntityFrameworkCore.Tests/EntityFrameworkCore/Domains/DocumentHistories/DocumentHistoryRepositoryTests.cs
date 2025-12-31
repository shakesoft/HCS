using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.DocumentHistories;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.DocumentHistories;

public class DocumentHistoryRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IDocumentHistoryRepository _documentHistoryRepository;

    public DocumentHistoryRepositoryTests()
    {
        _documentHistoryRepository = GetRequiredService<IDocumentHistoryRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _documentHistoryRepository.GetListAsync(comment: "c02b244edbb74c2680eb10bf451648cf3bd22e3f50574f36b50d13d7455f14ce6c8f4c93757b4", action: "9ee45603c1e14f30a1adf649fb9706");
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("1a52ebe0-8da8-446e-9fa5-19c062b5fe6c"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _documentHistoryRepository.GetCountAsync(comment: "45922b68640d44a7902e02", action: "072541cc4edd4baca3f26469a52991");
            // Assert
            result.ShouldBe(1);
        });
    }
}