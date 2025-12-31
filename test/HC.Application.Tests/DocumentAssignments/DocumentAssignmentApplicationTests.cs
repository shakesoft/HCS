using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.DocumentAssignments;

public abstract class DocumentAssignmentsAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IDocumentAssignmentsAppService _documentAssignmentsAppService;
    private readonly IRepository<DocumentAssignment, Guid> _documentAssignmentRepository;

    public DocumentAssignmentsAppServiceTests()
    {
        _documentAssignmentsAppService = GetRequiredService<IDocumentAssignmentsAppService>();
        _documentAssignmentRepository = GetRequiredService<IRepository<DocumentAssignment, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _documentAssignmentsAppService.GetListAsync(new GetDocumentAssignmentsInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.DocumentAssignment.Id == Guid.Parse("249be687-15b9-4841-8341-a80f6c606a49")).ShouldBe(true);
        result.Items.Any(x => x.DocumentAssignment.Id == Guid.Parse("6eb26e9e-1d79-41f6-8b37-59ccf2803923")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _documentAssignmentsAppService.GetAsync(Guid.Parse("249be687-15b9-4841-8341-a80f6c606a49"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("249be687-15b9-4841-8341-a80f6c606a49"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new DocumentAssignmentCreateDto
        {
            StepOrder = 12,
            ActionType = "4a9e43434e09406f8524",
            Status = "2e58aba4456b46d6b457",
            AssignedAt = new DateTime(2024, 8, 17),
            ProcessedAt = new DateTime(2019, 2, 20),
            IsCurrent = true,
            DocumentId = ,
            StepId = ,
            ReceiverUserId =
        };
        // Act
        var serviceResult = await _documentAssignmentsAppService.CreateAsync(input);
        // Assert
        var result = await _documentAssignmentRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.StepOrder.ShouldBe(12);
        result.ActionType.ShouldBe("4a9e43434e09406f8524");
        result.Status.ShouldBe("2e58aba4456b46d6b457");
        result.AssignedAt.ShouldBe(new DateTime(2024, 8, 17));
        result.ProcessedAt.ShouldBe(new DateTime(2019, 2, 20));
        result.IsCurrent.ShouldBe(true);
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new DocumentAssignmentUpdateDto()
        {
            StepOrder = 7,
            ActionType = "c85471669890494c8f5a",
            Status = "33215c907eda49d5ae73",
            AssignedAt = new DateTime(2024, 9, 25),
            ProcessedAt = new DateTime(2009, 8, 25),
            IsCurrent = true,
            DocumentId = ,
            StepId = ,
            ReceiverUserId =
        };
        // Act
        var serviceResult = await _documentAssignmentsAppService.UpdateAsync(Guid.Parse("249be687-15b9-4841-8341-a80f6c606a49"), input);
        // Assert
        var result = await _documentAssignmentRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.StepOrder.ShouldBe(7);
        result.ActionType.ShouldBe("c85471669890494c8f5a");
        result.Status.ShouldBe("33215c907eda49d5ae73");
        result.AssignedAt.ShouldBe(new DateTime(2024, 9, 25));
        result.ProcessedAt.ShouldBe(new DateTime(2009, 8, 25));
        result.IsCurrent.ShouldBe(true);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _documentAssignmentsAppService.DeleteAsync(Guid.Parse("249be687-15b9-4841-8341-a80f6c606a49"));
        // Assert
        var result = await _documentAssignmentRepository.FindAsync(c => c.Id == Guid.Parse("249be687-15b9-4841-8341-a80f6c606a49"));
        result.ShouldBeNull();
    }
}