using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.WorkflowDefinitions;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.WorkflowDefinitions;

public class WorkflowDefinitionRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;

    public WorkflowDefinitionRepositoryTests()
    {
        _workflowDefinitionRepository = GetRequiredService<IWorkflowDefinitionRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _workflowDefinitionRepository.GetListAsync(code: "771b812b72c8432ca2ac0464bd2e9689bf59053399de45c6af", name: "3b8c325064fa42a78956d5e9ec8a227cd2be1a9", description: "57b5c043564b42bfaa7b2f8", isActive: true);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("0e604ee9-61cf-473d-baf9-a02f14e32d58"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _workflowDefinitionRepository.GetCountAsync(code: "c8b258e8e445493d9a31e068b6603576cdf1345c97e54f478b", name: "ace27cf471ea45eeaf4f35deff8f0915a641bd27867e4cd091d6b41fec5920dfaaba708f6bc5484ea039f0", description: "1d8f35a768604be683d7b7a321a813b0782", isActive: true);
            // Assert
            result.ShouldBe(1);
        });
    }
}