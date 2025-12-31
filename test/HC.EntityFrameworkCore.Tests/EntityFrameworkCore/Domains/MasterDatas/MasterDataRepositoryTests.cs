using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.MasterDatas;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.MasterDatas;

public class MasterDataRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IMasterDataRepository _masterDataRepository;

    public MasterDataRepositoryTests()
    {
        _masterDataRepository = GetRequiredService<IMasterDataRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _masterDataRepository.GetListAsync(type: "8f0607d6a74d4d66a201ff2d492bdd6d293a4770d3e14eabb9", code: "837a777093ff47548bb9b6ed2efe0a2137f3b795c5714f8da6", name: "77b60d6806af4dbe9e78261c5dec0ff47", isActive: true);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("12feb6c5-7d61-44a9-b5df-3e194308c1dc"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _masterDataRepository.GetCountAsync(type: "a889e92e75514f06ae98bc69be27b918ce79e9c4c22b48d3ad", code: "86377e023d5f49a890a57dd2d8cbae221fb39e4cae8644a7b1", name: "fcbcde00975b43909e5b7dc729501", isActive: true);
            // Assert
            result.ShouldBe(1);
        });
    }
}