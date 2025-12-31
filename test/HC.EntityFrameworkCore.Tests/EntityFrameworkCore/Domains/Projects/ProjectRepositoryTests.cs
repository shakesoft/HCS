using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.Projects;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.Projects;

public class ProjectRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IProjectRepository _projectRepository;

    public ProjectRepositoryTests()
    {
        _projectRepository = GetRequiredService<IProjectRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _projectRepository.GetListAsync(code: "0063efc608df4591961b442b088f6522d906ab42ed3240aaa6", name: "53c52bf7c6204d01902e32418462e3ee77ecfe0fb10d42f6a3cc272eda5b47985bd1fec8499c4d24affc70017fbe4850377e795c32c24e2bb998547de822ea6f2f8326b916574d52b141084e62c581df6b913e8f40d8455198a0ca9803063129d81875d8413844f3ba13a5db04f02cafb8fbc3a1487e4e279aa8251ef902f89", description: "54bcf123a8f04ec5b9b2e93225390b0170f63940a4734c08af2bd28cda5aae092d124f85031c41aea30b1ef9aa9aa46f580", status: "39bf875c2fe64fb3aa960d9671c4df");
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("91c5c5d6-9cc3-48f4-9493-abcd2f87f5aa"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _projectRepository.GetCountAsync(code: "bc6315f517ab49e1a2fca17ef615c0f58e96ac8ac09c4ae599", name: "891f3ecf179442459fcef8117e3ed131e315af9f0681430191ee8a32c53cf35bb06df11c2cad4fccb37b4b7fd0d673068cb02e573cb64b9884e3a49024e413c9457773d981d3478eaf8e06741d8368a45cee65a21b0844f3a7b62de27974bc55b39a402e626440e1855b47d44853568fecf9ca93dd5448ff93cc94a9cfd10f8", description: "a4ff50254fd540b7a121c73cb9c88df5484", status: "94987326cb8247f7b76e4622c37c35");
            // Assert
            result.ShouldBe(1);
        });
    }
}