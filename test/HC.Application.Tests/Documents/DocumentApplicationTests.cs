using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.Documents;

public abstract class DocumentsAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IDocumentsAppService _documentsAppService;
    private readonly IRepository<Document, Guid> _documentRepository;

    public DocumentsAppServiceTests()
    {
        _documentsAppService = GetRequiredService<IDocumentsAppService>();
        _documentRepository = GetRequiredService<IRepository<Document, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _documentsAppService.GetListAsync(new GetDocumentsInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.Document.Id == Guid.Parse("af8c4d9b-85e8-4de0-9dd3-c40768b59e9c")).ShouldBe(true);
        result.Items.Any(x => x.Document.Id == Guid.Parse("510c0e51-2439-4c74-b85f-567ed4319cae")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _documentsAppService.GetAsync(Guid.Parse("af8c4d9b-85e8-4de0-9dd3-c40768b59e9c"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("af8c4d9b-85e8-4de0-9dd3-c40768b59e9c"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new DocumentCreateDto
        {
            No = "ded06dfd37634c3da0cf945c62e6b4e1657782cf822446c99a",
            Title = "2e0e0dac4092410a8a4cd995ab096145430c6479a28245da9c36097289d22668e7503f8c57664493b2aad629c6b",
            Type = "f84e7df3d5c845fa8b339b5945ec4ac7a6b44237c39a417794",
            UrgencyLevel = "13e112619cf64038863c",
            SecrecyLevel = "e5ebaa87d4a44af99d35",
            CurrentStatus = "ca9d6bddef34489aba02d9d19bd3e1",
            CompletedTime = new DateTime(2002, 9, 7)
        };
        // Act
        var serviceResult = await _documentsAppService.CreateAsync(input);
        // Assert
        var result = await _documentRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.No.ShouldBe("ded06dfd37634c3da0cf945c62e6b4e1657782cf822446c99a");
        result.Title.ShouldBe("2e0e0dac4092410a8a4cd995ab096145430c6479a28245da9c36097289d22668e7503f8c57664493b2aad629c6b");
        result.Type.ShouldBe("f84e7df3d5c845fa8b339b5945ec4ac7a6b44237c39a417794");
        result.UrgencyLevel.ShouldBe("13e112619cf64038863c");
        result.SecrecyLevel.ShouldBe("e5ebaa87d4a44af99d35");
        result.CurrentStatus.ShouldBe("ca9d6bddef34489aba02d9d19bd3e1");
        result.CompletedTime.ShouldBe(new DateTime(2002, 9, 7));
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new DocumentUpdateDto()
        {
            No = "788a41382240416ab51b2638f2abef1fc649be28ee594c479d",
            Title = "21176c96492d41cb8fa1bec6e45a71d12c120bbf80a747f5baa309a1c05fbc6e8931665c5b3b4969bd527776dcfb0019",
            Type = "a5fbcc352dae44e1813ca17db1833b370d41d2c529d5452f8b",
            UrgencyLevel = "e0b10a47ef804e52bb87",
            SecrecyLevel = "135ddfbd669c4d4b862a",
            CurrentStatus = "2dc67ff27bdd4176aa3b84f15213fe",
            CompletedTime = new DateTime(2016, 3, 4)
        };
        // Act
        var serviceResult = await _documentsAppService.UpdateAsync(Guid.Parse("af8c4d9b-85e8-4de0-9dd3-c40768b59e9c"), input);
        // Assert
        var result = await _documentRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.No.ShouldBe("788a41382240416ab51b2638f2abef1fc649be28ee594c479d");
        result.Title.ShouldBe("21176c96492d41cb8fa1bec6e45a71d12c120bbf80a747f5baa309a1c05fbc6e8931665c5b3b4969bd527776dcfb0019");
        result.Type.ShouldBe("a5fbcc352dae44e1813ca17db1833b370d41d2c529d5452f8b");
        result.UrgencyLevel.ShouldBe("e0b10a47ef804e52bb87");
        result.SecrecyLevel.ShouldBe("135ddfbd669c4d4b862a");
        result.CurrentStatus.ShouldBe("2dc67ff27bdd4176aa3b84f15213fe");
        result.CompletedTime.ShouldBe(new DateTime(2016, 3, 4));
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _documentsAppService.DeleteAsync(Guid.Parse("af8c4d9b-85e8-4de0-9dd3-c40768b59e9c"));
        // Assert
        var result = await _documentRepository.FindAsync(c => c.Id == Guid.Parse("af8c4d9b-85e8-4de0-9dd3-c40768b59e9c"));
        result.ShouldBeNull();
    }
}