using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.Departments;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.Departments;

public class DepartmentRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentRepositoryTests()
    {
        _departmentRepository = GetRequiredService<IDepartmentRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _departmentRepository.GetListAsync(code: "36ba02f7df374080b70cda8a16d3bf324b98ee224bdb42cbaf", name: "faa5c9b038a94a42", parentId: "f3cfc4d937a94917b8143b15d73f59692cd3806a279041af884cfcf0cc1e95f778f5b26e6af", isActive: true);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("cef08c27-8872-40ce-ad09-bec261cd8502"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _departmentRepository.GetCountAsync(code: "d93721fb195347deb9eb861744a0f69265d38f8c5041478f8e", name: "86a8a83da38249e9ad57858601a0d9503d44ed20cdb842028c", parentId: "cf9990bf5aac4becb34870", isActive: true);
            // Assert
            result.ShouldBe(1);
        });
    }
}