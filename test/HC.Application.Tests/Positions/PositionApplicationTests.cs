using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.Positions;

public abstract class PositionsAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IPositionsAppService _positionsAppService;
    private readonly IRepository<Position, Guid> _positionRepository;

    public PositionsAppServiceTests()
    {
        _positionsAppService = GetRequiredService<IPositionsAppService>();
        _positionRepository = GetRequiredService<IRepository<Position, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _positionsAppService.GetListAsync(new GetPositionsInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.Id == Guid.Parse("34fcc9a7-223a-4ddb-8754-571ec41cc6ae")).ShouldBe(true);
        result.Items.Any(x => x.Id == Guid.Parse("06d89d44-bd03-474b-aea8-dae068e6d17c")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _positionsAppService.GetAsync(Guid.Parse("34fcc9a7-223a-4ddb-8754-571ec41cc6ae"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("34fcc9a7-223a-4ddb-8754-571ec41cc6ae"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new PositionCreateDto
        {
            Code = "6acb3353748943dabed5139a2cd637501be17206512e446181",
            Name = "3ff44a18926542f4a3770f2301ac61e7baf7357c1ac142c4a6a4f9b4498e8e35cc4b66c680e7439384cc329b0ecee64cd20",
            SignOrder = 54,
            IsActive = true
        };
        // Act
        var serviceResult = await _positionsAppService.CreateAsync(input);
        // Assert
        var result = await _positionRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("6acb3353748943dabed5139a2cd637501be17206512e446181");
        result.Name.ShouldBe("3ff44a18926542f4a3770f2301ac61e7baf7357c1ac142c4a6a4f9b4498e8e35cc4b66c680e7439384cc329b0ecee64cd20");
        result.SignOrder.ShouldBe(54);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new PositionUpdateDto()
        {
            Code = "d9539d064a464f02b5467e1bff696e46502a9e89498e4b9488",
            Name = "c95dc0a4516d483f90",
            SignOrder = 18,
            IsActive = true
        };
        // Act
        var serviceResult = await _positionsAppService.UpdateAsync(Guid.Parse("34fcc9a7-223a-4ddb-8754-571ec41cc6ae"), input);
        // Assert
        var result = await _positionRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("d9539d064a464f02b5467e1bff696e46502a9e89498e4b9488");
        result.Name.ShouldBe("c95dc0a4516d483f90");
        result.SignOrder.ShouldBe(18);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _positionsAppService.DeleteAsync(Guid.Parse("34fcc9a7-223a-4ddb-8754-571ec41cc6ae"));
        // Assert
        var result = await _positionRepository.FindAsync(c => c.Id == Guid.Parse("34fcc9a7-223a-4ddb-8754-571ec41cc6ae"));
        result.ShouldBeNull();
    }
}