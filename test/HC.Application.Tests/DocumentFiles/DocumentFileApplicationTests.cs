using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.DocumentFiles;

public abstract class DocumentFilesAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IDocumentFilesAppService _documentFilesAppService;
    private readonly IRepository<DocumentFile, Guid> _documentFileRepository;

    public DocumentFilesAppServiceTests()
    {
        _documentFilesAppService = GetRequiredService<IDocumentFilesAppService>();
        _documentFileRepository = GetRequiredService<IRepository<DocumentFile, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _documentFilesAppService.GetListAsync(new GetDocumentFilesInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.DocumentFile.Id == Guid.Parse("cb5d2b5d-44e9-474b-b1f3-11965ee88c52")).ShouldBe(true);
        result.Items.Any(x => x.DocumentFile.Id == Guid.Parse("e089ae06-e0d1-49dc-9c9a-d9771cccb6e8")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _documentFilesAppService.GetAsync(Guid.Parse("cb5d2b5d-44e9-474b-b1f3-11965ee88c52"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("cb5d2b5d-44e9-474b-b1f3-11965ee88c52"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new DocumentFileCreateDto
        {
            Name = "b846e832dc4e4af995fc4143a05d3022a26f664b94034585bc2b48c6e2e582cb983d1deea8844a389bab56dbd05434",
            Path = "55830fd2d8f84523baa1076dadae3d36d6242192137c4503be70ee3",
            Hash = "066b6d9dd71b4b1c9ece0b2f4e38336767c86d0700614f7290410dbbb714bf1ef606826c97074f05bfc00c6b4a58a95baae",
            IsSigned = true,
            UploadedAt = new DateTime(2018, 8, 15),
            DocumentId =
        };
        // Act
        var serviceResult = await _documentFilesAppService.CreateAsync(input);
        // Assert
        var result = await _documentFileRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Name.ShouldBe("b846e832dc4e4af995fc4143a05d3022a26f664b94034585bc2b48c6e2e582cb983d1deea8844a389bab56dbd05434");
        result.Path.ShouldBe("55830fd2d8f84523baa1076dadae3d36d6242192137c4503be70ee3");
        result.Hash.ShouldBe("066b6d9dd71b4b1c9ece0b2f4e38336767c86d0700614f7290410dbbb714bf1ef606826c97074f05bfc00c6b4a58a95baae");
        result.IsSigned.ShouldBe(true);
        result.UploadedAt.ShouldBe(new DateTime(2018, 8, 15));
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new DocumentFileUpdateDto()
        {
            Name = "9344c79ed0e545dbac9bac696c172ceaf4b70f688e864f99937c4f3897",
            Path = "e8ebc7f7",
            Hash = "0146fb00d3be4820b42ef65201e3a63c918363fd8f8e4317bc452f090d91852500c335fa71c64d7",
            IsSigned = true,
            UploadedAt = new DateTime(2022, 5, 23),
            DocumentId =
        };
        // Act
        var serviceResult = await _documentFilesAppService.UpdateAsync(Guid.Parse("cb5d2b5d-44e9-474b-b1f3-11965ee88c52"), input);
        // Assert
        var result = await _documentFileRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Name.ShouldBe("9344c79ed0e545dbac9bac696c172ceaf4b70f688e864f99937c4f3897");
        result.Path.ShouldBe("e8ebc7f7");
        result.Hash.ShouldBe("0146fb00d3be4820b42ef65201e3a63c918363fd8f8e4317bc452f090d91852500c335fa71c64d7");
        result.IsSigned.ShouldBe(true);
        result.UploadedAt.ShouldBe(new DateTime(2022, 5, 23));
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _documentFilesAppService.DeleteAsync(Guid.Parse("cb5d2b5d-44e9-474b-b1f3-11965ee88c52"));
        // Assert
        var result = await _documentFileRepository.FindAsync(c => c.Id == Guid.Parse("cb5d2b5d-44e9-474b-b1f3-11965ee88c52"));
        result.ShouldBeNull();
    }
}