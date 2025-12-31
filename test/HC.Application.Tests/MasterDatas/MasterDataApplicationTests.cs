using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.MasterDatas;

public abstract class MasterDatasAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IMasterDatasAppService _masterDatasAppService;
    private readonly IRepository<MasterData, Guid> _masterDataRepository;

    public MasterDatasAppServiceTests()
    {
        _masterDatasAppService = GetRequiredService<IMasterDatasAppService>();
        _masterDataRepository = GetRequiredService<IRepository<MasterData, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _masterDatasAppService.GetListAsync(new GetMasterDatasInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.Id == Guid.Parse("12feb6c5-7d61-44a9-b5df-3e194308c1dc")).ShouldBe(true);
        result.Items.Any(x => x.Id == Guid.Parse("e626b30d-fe62-43dc-ad24-ad0f2ea6d3cc")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _masterDatasAppService.GetAsync(Guid.Parse("12feb6c5-7d61-44a9-b5df-3e194308c1dc"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("12feb6c5-7d61-44a9-b5df-3e194308c1dc"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new MasterDataCreateDto
        {
            Type = "74f2ed9f604a45f2a95e0af22a0ebb0599c6792742b94f7daa",
            Code = "6bf1fef854fa44ac8a881cf875b86c2d42f5067422b84b3090",
            Name = "ef3a219b2f604f1c9266daae0d1b3aac054247550b974eb386e55322a449974515128693315c427dba14f135c1f71f",
            SortOrder = 1418,
            IsActive = true
        };
        // Act
        var serviceResult = await _masterDatasAppService.CreateAsync(input);
        // Assert
        var result = await _masterDataRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Type.ShouldBe("74f2ed9f604a45f2a95e0af22a0ebb0599c6792742b94f7daa");
        result.Code.ShouldBe("6bf1fef854fa44ac8a881cf875b86c2d42f5067422b84b3090");
        result.Name.ShouldBe("ef3a219b2f604f1c9266daae0d1b3aac054247550b974eb386e55322a449974515128693315c427dba14f135c1f71f");
        result.SortOrder.ShouldBe(1418);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new MasterDataUpdateDto()
        {
            Type = "b22f279b5acd4e22a9e3cfdba69c82df17118edcc6114e3d95",
            Code = "001a23fb8a824112a87648dcce95adef7a3f5da97762445197",
            Name = "6cd2c5",
            SortOrder = 5270,
            IsActive = true
        };
        // Act
        var serviceResult = await _masterDatasAppService.UpdateAsync(Guid.Parse("12feb6c5-7d61-44a9-b5df-3e194308c1dc"), input);
        // Assert
        var result = await _masterDataRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Type.ShouldBe("b22f279b5acd4e22a9e3cfdba69c82df17118edcc6114e3d95");
        result.Code.ShouldBe("001a23fb8a824112a87648dcce95adef7a3f5da97762445197");
        result.Name.ShouldBe("6cd2c5");
        result.SortOrder.ShouldBe(5270);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _masterDatasAppService.DeleteAsync(Guid.Parse("12feb6c5-7d61-44a9-b5df-3e194308c1dc"));
        // Assert
        var result = await _masterDataRepository.FindAsync(c => c.Id == Guid.Parse("12feb6c5-7d61-44a9-b5df-3e194308c1dc"));
        result.ShouldBeNull();
    }
}