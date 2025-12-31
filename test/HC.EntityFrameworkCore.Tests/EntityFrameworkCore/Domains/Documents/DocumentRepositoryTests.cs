using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.Documents;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.Documents;

public class DocumentRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IDocumentRepository _documentRepository;

    public DocumentRepositoryTests()
    {
        _documentRepository = GetRequiredService<IDocumentRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _documentRepository.GetListAsync(no: "5a5922bb89b24e77bd043d1b57e7afbf63a778306a27497cbc", title: "c0a3bcf33ce14b68ba16ad6412a6595d9e6704ff4e9d46299bf675b515359ddfd95bb", type: "d617f936da2144ad9bc12371357df84a52368073ca0941389d", urgencyLevel: "aeb4b5104fba49d6aff9", secrecyLevel: "919cda906cd04731a64a", currentStatus: "5541555bf3da45e983f43f4bbe7442");
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("af8c4d9b-85e8-4de0-9dd3-c40768b59e9c"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _documentRepository.GetCountAsync(no: "8ff5bb65fdb7403b9d61e93749340fac6580cec65d1e4fc093", title: "0844145422ed4e5c8ae278d46e73217f085eac81e68e43429dbc8f5c97778d", type: "da7ef1a704bf4ca38c44d96de3ceaa3d0e3a17a4ffd5450dbe", urgencyLevel: "8aad68edcea149919b0c", secrecyLevel: "de9c3b5c2c2f47058f39", currentStatus: "730c7a91a9da440aa25f02b8017191");
            // Assert
            result.ShouldBe(1);
        });
    }
}