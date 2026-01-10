using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.Projects;

public abstract class ProjectsAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IProjectsAppService _projectsAppService;
    private readonly IRepository<Project, Guid> _projectRepository;

    public ProjectsAppServiceTests()
    {
        _projectsAppService = GetRequiredService<IProjectsAppService>();
        _projectRepository = GetRequiredService<IRepository<Project, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _projectsAppService.GetListAsync(new GetProjectsInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.Project.Id == Guid.Parse("f89a627c-d03e-40d2-9e34-c9826b5a7d37")).ShouldBe(true);
        result.Items.Any(x => x.Project.Id == Guid.Parse("76264b58-2bbf-4ecc-b457-7a629b423e9c")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _projectsAppService.GetAsync(Guid.Parse("f89a627c-d03e-40d2-9e34-c9826b5a7d37"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("f89a627c-d03e-40d2-9e34-c9826b5a7d37"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new ProjectCreateDto
        {
            Code = "1ad3fcfefdb342309b75f5bd84166c5ff5f60758407e468d87",
            Name = "e0c71e666cd642438feb145bd8d035d194ac87bf0da74cd38e5e2ca84b819ae0f2110823650a499d913c34cd216ad6c27ea19aca46ed45ccb8e461b3b29821a05e050ea69b474e6f9398ddc01e414bb02873b2a074504d2a9b2979bf5686acdcf4c5b25deb6c4a9ea242e218b6005d4e0bf04092f8044354a2f0ef68c5622fe",
            Description = "774c6740185b444e82",
            StartDate = new DateTime(2012, 10, 18),
            EndDate = new DateTime(2000, 10, 14),
            Status = "8238f0f395c845c3bfb07a8347a08b02135ae60581d74532a2fae5a699eabdbedf86a0d2f46044a2bcd53"
        };
        // Act
        var serviceResult = await _projectsAppService.CreateAsync(input);
        // Assert
        var result = await _projectRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("1ad3fcfefdb342309b75f5bd84166c5ff5f60758407e468d87");
        result.Name.ShouldBe("e0c71e666cd642438feb145bd8d035d194ac87bf0da74cd38e5e2ca84b819ae0f2110823650a499d913c34cd216ad6c27ea19aca46ed45ccb8e461b3b29821a05e050ea69b474e6f9398ddc01e414bb02873b2a074504d2a9b2979bf5686acdcf4c5b25deb6c4a9ea242e218b6005d4e0bf04092f8044354a2f0ef68c5622fe");
        result.Description.ShouldBe("774c6740185b444e82");
        result.StartDate.ShouldBe(new DateTime(2012, 10, 18));
        result.EndDate.ShouldBe(new DateTime(2000, 10, 14));
        result.Status.ShouldBe("8238f0f395c845c3bfb07a8347a08b02135ae60581d74532a2fae5a699eabdbedf86a0d2f46044a2bcd53");
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new ProjectUpdateDto()
        {
            Code = "d6b2541910dd43859e39b5d7402340cf3a79e769f8da4f6b9c",
            Name = "9d4fa1909b824b8eb0fe9f659fd5e741c594c23fd78a4abd909aa20b9d01fa84c7b6b619675943c2874dd3963aa51db9f285cad771dc44168cf16b02e25f632dc268c6efd7e7462085791e4e4438e2923b2ecc4492ea4ff9b10a82f6e257a10ea33ccfc160154524b94a547ff6257243a8ffecc660ef420eb1c656c3f67b645",
            Description = "2ef3586c245c418b89b93d41ce709529e6f7fdfca9db4",
            StartDate = new DateTime(2004, 2, 2),
            EndDate = new DateTime(2007, 6, 17),
            Status = "586cc30"
        };
        // Act
        var serviceResult = await _projectsAppService.UpdateAsync(Guid.Parse("f89a627c-d03e-40d2-9e34-c9826b5a7d37"), input);
        // Assert
        var result = await _projectRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("d6b2541910dd43859e39b5d7402340cf3a79e769f8da4f6b9c");
        result.Name.ShouldBe("9d4fa1909b824b8eb0fe9f659fd5e741c594c23fd78a4abd909aa20b9d01fa84c7b6b619675943c2874dd3963aa51db9f285cad771dc44168cf16b02e25f632dc268c6efd7e7462085791e4e4438e2923b2ecc4492ea4ff9b10a82f6e257a10ea33ccfc160154524b94a547ff6257243a8ffecc660ef420eb1c656c3f67b645");
        result.Description.ShouldBe("2ef3586c245c418b89b93d41ce709529e6f7fdfca9db4");
        result.StartDate.ShouldBe(new DateTime(2004, 2, 2));
        result.EndDate.ShouldBe(new DateTime(2007, 6, 17));
        result.Status.ShouldBe("586cc30");
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _projectsAppService.DeleteAsync(Guid.Parse("f89a627c-d03e-40d2-9e34-c9826b5a7d37"));
        // Assert
        var result = await _projectRepository.FindAsync(c => c.Id == Guid.Parse("f89a627c-d03e-40d2-9e34-c9826b5a7d37"));
        result.ShouldBeNull();
    }
}