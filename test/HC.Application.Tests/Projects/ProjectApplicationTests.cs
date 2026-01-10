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
        result.Items.Any(x => x.Project.Id == Guid.Parse("4b11cb6c-18c9-40b4-9baf-dacb948a5d88")).ShouldBe(true);
        result.Items.Any(x => x.Project.Id == Guid.Parse("da6f6522-d787-4df8-b903-44c0a5ab6c5b")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _projectsAppService.GetAsync(Guid.Parse("4b11cb6c-18c9-40b4-9baf-dacb948a5d88"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("4b11cb6c-18c9-40b4-9baf-dacb948a5d88"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new ProjectCreateDto
        {
            Code = "19acec055f81462eb80701f37863704a3e45f16515a2416b80",
            Name = "36140478f09642a889ba4631ca2ab2753721bfe395234e89bf470e4ac34e5842e1df9ee5c2564f5bb5c74617dd51d58dda6e7e974bd84c7ca90348e0271e6302db49da1efb6842fa90b87e786697c689977a9a21eef44133b1813391fa012721695fbf7ce2bf46b680d39bd3c6c9c14d299b3a3867884740bf6e2c3f6baeee5",
            Description = "f026784c098b4ad7adb4a807086b",
            StartDate = new DateTime(2003, 10, 8),
            EndDate = new DateTime(2024, 6, 17),
            Status = default
        };
        // Act
        var serviceResult = await _projectsAppService.CreateAsync(input);
        // Assert
        var result = await _projectRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("19acec055f81462eb80701f37863704a3e45f16515a2416b80");
        result.Name.ShouldBe("36140478f09642a889ba4631ca2ab2753721bfe395234e89bf470e4ac34e5842e1df9ee5c2564f5bb5c74617dd51d58dda6e7e974bd84c7ca90348e0271e6302db49da1efb6842fa90b87e786697c689977a9a21eef44133b1813391fa012721695fbf7ce2bf46b680d39bd3c6c9c14d299b3a3867884740bf6e2c3f6baeee5");
        result.Description.ShouldBe("f026784c098b4ad7adb4a807086b");
        result.StartDate.ShouldBe(new DateTime(2003, 10, 8));
        result.EndDate.ShouldBe(new DateTime(2024, 6, 17));
        result.Status.ShouldBe(default);
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new ProjectUpdateDto()
        {
            Code = "92e78e3f92c44b82a05f64304c185e86afa072b6c27f4b6ba0",
            Name = "4a21e7c8bd4e444a90cd1e89216315cc2a1048e6fb9e4b63ab8a35664f5f167e91cd36a210064c2197c886a4540299721f196ca1e5cc425c9e3114e2f97c8193e6b394b41bc24f27b46fb809cba64b6da6ddb19e1c9f4588a72bc2b230a375194cc3f15988c1478cafea501136eff5bfec5d4f3e8c3c4788bbbc16c9d6b453c",
            Description = "6cfee843d0e841b39feadd2448d6e7be7",
            StartDate = new DateTime(2019, 6, 8),
            EndDate = new DateTime(2011, 4, 10),
            Status = default
        };
        // Act
        var serviceResult = await _projectsAppService.UpdateAsync(Guid.Parse("4b11cb6c-18c9-40b4-9baf-dacb948a5d88"), input);
        // Assert
        var result = await _projectRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("92e78e3f92c44b82a05f64304c185e86afa072b6c27f4b6ba0");
        result.Name.ShouldBe("4a21e7c8bd4e444a90cd1e89216315cc2a1048e6fb9e4b63ab8a35664f5f167e91cd36a210064c2197c886a4540299721f196ca1e5cc425c9e3114e2f97c8193e6b394b41bc24f27b46fb809cba64b6da6ddb19e1c9f4588a72bc2b230a375194cc3f15988c1478cafea501136eff5bfec5d4f3e8c3c4788bbbc16c9d6b453c");
        result.Description.ShouldBe("6cfee843d0e841b39feadd2448d6e7be7");
        result.StartDate.ShouldBe(new DateTime(2019, 6, 8));
        result.EndDate.ShouldBe(new DateTime(2011, 4, 10));
        result.Status.ShouldBe(default);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _projectsAppService.DeleteAsync(Guid.Parse("4b11cb6c-18c9-40b4-9baf-dacb948a5d88"));
        // Assert
        var result = await _projectRepository.FindAsync(c => c.Id == Guid.Parse("4b11cb6c-18c9-40b4-9baf-dacb948a5d88"));
        result.ShouldBeNull();
    }
}