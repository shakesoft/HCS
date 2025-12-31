using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.Units;

public abstract class UnitsAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IUnitsAppService _unitsAppService;
    private readonly IRepository<Unit, Guid> _unitRepository;

    public UnitsAppServiceTests()
    {
        _unitsAppService = GetRequiredService<IUnitsAppService>();
        _unitRepository = GetRequiredService<IRepository<Unit, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _unitsAppService.GetListAsync(new GetUnitsInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.Id == Guid.Parse("25b91d47-2958-4b3c-8d97-23ffa17a5632")).ShouldBe(true);
        result.Items.Any(x => x.Id == Guid.Parse("26b63ee5-972f-43e8-8e7a-e59130deacc5")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _unitsAppService.GetAsync(Guid.Parse("25b91d47-2958-4b3c-8d97-23ffa17a5632"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("25b91d47-2958-4b3c-8d97-23ffa17a5632"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new UnitCreateDto
        {
            Code = "dc450e52eff941e8ae720d41156263498fc3c3e3d7b04da691",
            Name = "777c107f846e41aa8a923ff294cce8d51967a",
            SortOrder = 2086885147,
            IsActive = true
        };
        // Act
        var serviceResult = await _unitsAppService.CreateAsync(input);
        // Assert
        var result = await _unitRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("dc450e52eff941e8ae720d41156263498fc3c3e3d7b04da691");
        result.Name.ShouldBe("777c107f846e41aa8a923ff294cce8d51967a");
        result.SortOrder.ShouldBe(2086885147);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new UnitUpdateDto()
        {
            Code = "da681130be2945f1b8a5e4155cdf7a2739dd4c3aed244283a1",
            Name = "69491a39c55745ad94fac6b8d164172167475c60869d4c5fa087d95c296d751149d1ba",
            SortOrder = 828142014,
            IsActive = true
        };
        // Act
        var serviceResult = await _unitsAppService.UpdateAsync(Guid.Parse("25b91d47-2958-4b3c-8d97-23ffa17a5632"), input);
        // Assert
        var result = await _unitRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("da681130be2945f1b8a5e4155cdf7a2739dd4c3aed244283a1");
        result.Name.ShouldBe("69491a39c55745ad94fac6b8d164172167475c60869d4c5fa087d95c296d751149d1ba");
        result.SortOrder.ShouldBe(828142014);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _unitsAppService.DeleteAsync(Guid.Parse("25b91d47-2958-4b3c-8d97-23ffa17a5632"));
        // Assert
        var result = await _unitRepository.FindAsync(c => c.Id == Guid.Parse("25b91d47-2958-4b3c-8d97-23ffa17a5632"));
        result.ShouldBeNull();
    }
}