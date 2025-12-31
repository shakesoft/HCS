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
        result.Items.Any(x => x.Project.Id == Guid.Parse("91c5c5d6-9cc3-48f4-9493-abcd2f87f5aa")).ShouldBe(true);
        result.Items.Any(x => x.Project.Id == Guid.Parse("f23ede1a-0352-4f61-9aec-90325c1b2511")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _projectsAppService.GetAsync(Guid.Parse("91c5c5d6-9cc3-48f4-9493-abcd2f87f5aa"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("91c5c5d6-9cc3-48f4-9493-abcd2f87f5aa"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new ProjectCreateDto
        {
            Code = "6fbf804a37e84f9189935aafc985b48f9d166a516ad5420780",
            Name = "19f9c05f2bd94e82a2655df64c81962b9509cb0bbc544238b4932cf08bc43f46d125e6ca143247238417c865780fe2a88871306e278e414ca37b1505d37014737fb077f473464e748ecc5f489ab7d6ba700047a5a446484d944dacfd9c1c407598aa404cb0094642a85838767bbeec60f9d8614ddf1b446dbe29c2a97a2582d",
            Description = "8503d5513b87428090a7dbfc642d2c9fd6877b858744486994ffb02",
            StartDate = new DateTime(2012, 7, 27),
            EndDate = new DateTime(2023, 11, 14),
            Status = "fd67ab78be8e454fb24301cd110aa3"
        };
        // Act
        var serviceResult = await _projectsAppService.CreateAsync(input);
        // Assert
        var result = await _projectRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("6fbf804a37e84f9189935aafc985b48f9d166a516ad5420780");
        result.Name.ShouldBe("19f9c05f2bd94e82a2655df64c81962b9509cb0bbc544238b4932cf08bc43f46d125e6ca143247238417c865780fe2a88871306e278e414ca37b1505d37014737fb077f473464e748ecc5f489ab7d6ba700047a5a446484d944dacfd9c1c407598aa404cb0094642a85838767bbeec60f9d8614ddf1b446dbe29c2a97a2582d");
        result.Description.ShouldBe("8503d5513b87428090a7dbfc642d2c9fd6877b858744486994ffb02");
        result.StartDate.ShouldBe(new DateTime(2012, 7, 27));
        result.EndDate.ShouldBe(new DateTime(2023, 11, 14));
        result.Status.ShouldBe("fd67ab78be8e454fb24301cd110aa3");
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new ProjectUpdateDto()
        {
            Code = "48b23bf15e924fd6a568a16fa0820314aa8da8ad5d7240dda1",
            Name = "c8c5f7dd9ff54ffea5cfb5ae098100cb928f39bd69d046bca7e1c89e02fd461c84695f9216924b4ebebbfe0fd9f5629a7bd4e9c180864f1c9296c8f1c336d06cd106dbc679ed49d7b96b3811073af9c05c6d1fd4d33c42f5844ea5eee131f0bb79c81d2a78a942a0af7b985627545abb18cc544de2c543d7a5f9fb9f9f80ab0",
            Description = "28b49a5d23db4daea2267fa98563cb297a0a394f80b94485b3f8fa046311a8c20768a278229b42d6a2fc667de8ad4485",
            StartDate = new DateTime(2012, 10, 14),
            EndDate = new DateTime(2010, 5, 23),
            Status = "b021f0ed5a8e49f082f6a6324a30d7"
        };
        // Act
        var serviceResult = await _projectsAppService.UpdateAsync(Guid.Parse("91c5c5d6-9cc3-48f4-9493-abcd2f87f5aa"), input);
        // Assert
        var result = await _projectRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.Code.ShouldBe("48b23bf15e924fd6a568a16fa0820314aa8da8ad5d7240dda1");
        result.Name.ShouldBe("c8c5f7dd9ff54ffea5cfb5ae098100cb928f39bd69d046bca7e1c89e02fd461c84695f9216924b4ebebbfe0fd9f5629a7bd4e9c180864f1c9296c8f1c336d06cd106dbc679ed49d7b96b3811073af9c05c6d1fd4d33c42f5844ea5eee131f0bb79c81d2a78a942a0af7b985627545abb18cc544de2c543d7a5f9fb9f9f80ab0");
        result.Description.ShouldBe("28b49a5d23db4daea2267fa98563cb297a0a394f80b94485b3f8fa046311a8c20768a278229b42d6a2fc667de8ad4485");
        result.StartDate.ShouldBe(new DateTime(2012, 10, 14));
        result.EndDate.ShouldBe(new DateTime(2010, 5, 23));
        result.Status.ShouldBe("b021f0ed5a8e49f082f6a6324a30d7");
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _projectsAppService.DeleteAsync(Guid.Parse("91c5c5d6-9cc3-48f4-9493-abcd2f87f5aa"));
        // Assert
        var result = await _projectRepository.FindAsync(c => c.Id == Guid.Parse("91c5c5d6-9cc3-48f4-9493-abcd2f87f5aa"));
        result.ShouldBeNull();
    }
}