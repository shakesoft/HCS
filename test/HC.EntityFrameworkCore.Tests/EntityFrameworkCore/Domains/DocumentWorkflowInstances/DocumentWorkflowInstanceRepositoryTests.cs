using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.DocumentWorkflowInstances;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.DocumentWorkflowInstances;

public class DocumentWorkflowInstanceRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IDocumentWorkflowInstanceRepository _documentWorkflowInstanceRepository;

    public DocumentWorkflowInstanceRepositoryTests()
    {
        _documentWorkflowInstanceRepository = GetRequiredService<IDocumentWorkflowInstanceRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _documentWorkflowInstanceRepository.GetListAsync(status: "b7817d5648ea42ef9855");
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("10bc60a8-1b25-4c98-be34-4237f3cbabe6"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _documentWorkflowInstanceRepository.GetCountAsync(status: "e24e7cbe69ba4c25a078");
            // Assert
            result.ShouldBe(1);
        });
    }
}