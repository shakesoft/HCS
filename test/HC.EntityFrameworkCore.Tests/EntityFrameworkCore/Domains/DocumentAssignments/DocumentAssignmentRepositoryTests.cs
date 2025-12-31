using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.DocumentAssignments;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.DocumentAssignments;

public class DocumentAssignmentRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IDocumentAssignmentRepository _documentAssignmentRepository;

    public DocumentAssignmentRepositoryTests()
    {
        _documentAssignmentRepository = GetRequiredService<IDocumentAssignmentRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _documentAssignmentRepository.GetListAsync(actionType: "6c1c37b4ab724ad99504", status: "d7964f83dd534915bf57", isCurrent: true);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("249be687-15b9-4841-8341-a80f6c606a49"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _documentAssignmentRepository.GetCountAsync(actionType: "eba81a1b7fb34a7cb367", status: "e0a014a7efe54fdd810b", isCurrent: true);
            // Assert
            result.ShouldBe(1);
        });
    }
}