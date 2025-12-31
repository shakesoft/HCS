using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.WorkflowTemplates;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.WorkflowTemplates;

public class WorkflowTemplateRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IWorkflowTemplateRepository _workflowTemplateRepository;

    public WorkflowTemplateRepositoryTests()
    {
        _workflowTemplateRepository = GetRequiredService<IWorkflowTemplateRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _workflowTemplateRepository.GetListAsync(code: "d7fae1884d9a4f91bdd34711bb71bfabec91adfeaf994f34be", name: "c7e676cc81c0460883f24982d6fb0ce7d71ce89501c64dff80e5a1c36ef07c880a84f8023f9a45c9a8fa762c772af", outputFormat: "57fcfb2b30f2444f8d45");
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("42ef75ac-fa52-448b-baae-26c32dcc7c76"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _workflowTemplateRepository.GetCountAsync(code: "4bd6ec4036d34abcacab72ebc7f661b6854ffd4bad8e4e94bb", name: "239097600866466eb1f6fcb2175e97f4badd9b997663473689792609cda6fabf1adbe99632df4da584d6324fc5bdb3d", outputFormat: "f37fc70db56b487f936e");
            // Assert
            result.ShouldBe(1);
        });
    }
}