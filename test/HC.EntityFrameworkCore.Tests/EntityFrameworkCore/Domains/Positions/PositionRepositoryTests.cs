using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.Positions;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.Positions;

public class PositionRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IPositionRepository _positionRepository;

    public PositionRepositoryTests()
    {
        _positionRepository = GetRequiredService<IPositionRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _positionRepository.GetListAsync(code: "5f5a0638a6454cfc955d419883aab75c155dc1feced545eb92", name: "6f07005925ec4d57a4b64b0ddd3", isActive: true);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("34fcc9a7-223a-4ddb-8754-571ec41cc6ae"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _positionRepository.GetCountAsync(code: "4252f1d0e4524747b4774e84ff1c1cf598ca7d171627422d8a", name: "d74f6bc846dc4d898f0300de260aa0eb02569f494a534a2b811dded8af31a6f5dab5bc6f66", isActive: true);
            // Assert
            result.ShouldBe(1);
        });
    }
}