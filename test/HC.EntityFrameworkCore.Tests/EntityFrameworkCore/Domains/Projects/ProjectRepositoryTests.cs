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
            var result = await _projectRepository.GetListAsync(code: "9b8babb57d7c489696436b002151d9cf5ad26d70895b4e83a1", name: "bce877e523f14e8abcde45f67af5eef93376bee47f324ba49c62b3eed892be667883deea68c04942953546d661a773c688dc276883ff42c19cc57766b48f564643afa25136ec49d996385c30fcf4e29d0c24a97e1e124b9b881ac8299ccf5ad777712ea4f9f6456aba62433a8fc69e2f836c7aa287f74e91b1f872c9d18aa93", description: "a28117b024c44718a8bcea40c53c9b0aab9f4245e80941678c7a9f97607b993b", status: default);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("4b11cb6c-18c9-40b4-9baf-dacb948a5d88"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _projectRepository.GetCountAsync(code: "980caee0d6b742699bd10f95bc70c2eb64e430eb1f7c405898", name: "72cbad25652c464e884dd119593b3f8e10931826fc304589b2b3d7a7bf4a5f084dffb5e7f85a4ea9b59336e235fdcade2b4d95b4b34e4cacaabd3976d768cb95fbd2c8cc4e7c47f4b1a71e6376b3bdb9ce5ee4548b924d0eaedba551928c518cee9d40ee8bcc4efcb6bf8037766eda3d93fae9e344034118a1d54197e85d298", description: "77efd4c5aa354af28a20eed97e36649fb4096fc3e2", status: default);
            // Assert
            result.ShouldBe(1);
        });
    }
}