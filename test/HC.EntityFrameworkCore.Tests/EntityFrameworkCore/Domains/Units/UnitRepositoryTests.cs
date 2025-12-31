using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.Units;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.Units;

public class UnitRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IUnitRepository _unitRepository;

    public UnitRepositoryTests()
    {
        _unitRepository = GetRequiredService<IUnitRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _unitRepository.GetListAsync(code: "d06b26139659417a901f35173beb9ff32b0c5d1f5999463da7", name: "4141b3dabacf4d9c", isActive: true);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("25b91d47-2958-4b3c-8d97-23ffa17a5632"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _unitRepository.GetCountAsync(code: "9d04e57ddc2d4320a248761de2ee50276ad4c0a891bb4d4795", name: "521b2d73e52840ce82a52ed801aebc1", isActive: true);
            // Assert
            result.ShouldBe(1);
        });
    }
}