using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.Departments;

public abstract class DepartmentsAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IDepartmentsAppService _departmentsAppService;
    private readonly IRepository<Department, Guid> _departmentRepository;

    public DepartmentsAppServiceTests()
    {
        _departmentsAppService = GetRequiredService<IDepartmentsAppService>();
        _departmentRepository = GetRequiredService<IRepository<Department, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _departmentsAppService.GetListAsync(new GetDepartmentsInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.Department.Id == Guid.Parse("cef08c27-8872-40ce-ad09-bec261cd8502")).ShouldBe(true);
        result.Items.Any(x => x.Department.Id == Guid.Parse("28e385d1-ee7d-477f-949a-c94b2ef206d8")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _departmentsAppService.GetAsync(Guid.Parse("cef08c27-8872-40ce-ad09-bec261cd8502"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("cef08c27-8872-40ce-ad09-bec261cd8502"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new DepartmentCreateDto
        {
            Code = "34766cd3f93641daa9747435d94614fcbb1e00a50d31445290",
            Name = "c4fff3b58d754c0f81139dcc717a1c0582fef470dffb445290b7bce3f774a8f13ddee4f6b3fd4f5",
            ParentId = "87bb223ebecd49abbd551e0aa905f0c3bdcec0af0f3d47c8a14306607a6ed32",
            Level = 1550195110,
            SortOrder = 706333444,
            IsActive = true
        };
        // Act
        var serviceResult = await _departmentsAppService.CreateAsync(input);
        // Assert
        var result = await _departmentRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("34766cd3f93641daa9747435d94614fcbb1e00a50d31445290");
        result.Name.ShouldBe("c4fff3b58d754c0f81139dcc717a1c0582fef470dffb445290b7bce3f774a8f13ddee4f6b3fd4f5");
        result.ParentId.ShouldBe("87bb223ebecd49abbd551e0aa905f0c3bdcec0af0f3d47c8a14306607a6ed32");
        result.Level.ShouldBe(1550195110);
        result.SortOrder.ShouldBe(706333444);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new DepartmentUpdateDto()
        {
            Code = "5a7c960fe8174dcc83462f17c4370b04a2ce8abbb7cc4473b1",
            Name = "9199df78a59c4991acf5c7851872a5cdeed11ab8bfa0429ebb0ab7a8abcebb4ee",
            ParentId = "a3a0004d5e6e4331be44caf1eb214ce44b044e3ec4064fe782d02a240293575fc20a5b80e8ef4f50b70ec09d709238893",
            Level = 813095891,
            SortOrder = 1613293347,
            IsActive = true
        };
        // Act
        var serviceResult = await _departmentsAppService.UpdateAsync(Guid.Parse("cef08c27-8872-40ce-ad09-bec261cd8502"), input);
        // Assert
        var result = await _departmentRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("5a7c960fe8174dcc83462f17c4370b04a2ce8abbb7cc4473b1");
        result.Name.ShouldBe("9199df78a59c4991acf5c7851872a5cdeed11ab8bfa0429ebb0ab7a8abcebb4ee");
        result.ParentId.ShouldBe("a3a0004d5e6e4331be44caf1eb214ce44b044e3ec4064fe782d02a240293575fc20a5b80e8ef4f50b70ec09d709238893");
        result.Level.ShouldBe(813095891);
        result.SortOrder.ShouldBe(1613293347);
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _departmentsAppService.DeleteAsync(Guid.Parse("cef08c27-8872-40ce-ad09-bec261cd8502"));
        // Assert
        var result = await _departmentRepository.FindAsync(c => c.Id == Guid.Parse("cef08c27-8872-40ce-ad09-bec261cd8502"));
        result.ShouldBeNull();
    }
}