using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.Workflows;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.Workflows;

public class WorkflowRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IWorkflowRepository _workflowRepository;

    public WorkflowRepositoryTests()
    {
        _workflowRepository = GetRequiredService<IWorkflowRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _workflowRepository.GetListAsync(code: "81aef58849b7485eb9950c9d99f50cd3fee2a3eb68604888a6", name: "53fb3a1b7fd54964acee37309878c5aacd9c71227a63467bb89f4ebeeef64af669b3fd48a59d4d198ed3", description: "e2cd60d381e54d528ac0cc72f944c60bc9ab58b386b4483ba303ed96d22be132bb233a405317", isActive: true);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("66523a7a-3dcb-4880-a5c2-ae55ac3c4656"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _workflowRepository.GetCountAsync(code: "0192a12fc1534966af7a3b25994447a764747fa9e48e4317a2", name: "c99630793d6946c0960ee31a1f3b7ca", description: "c35eb022e2a744ccb870db2cc36c963b2a1cd8b", isActive: true);
            // Assert
            result.ShouldBe(1);
        });
    }
}