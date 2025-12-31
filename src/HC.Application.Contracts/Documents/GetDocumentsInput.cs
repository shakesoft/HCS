using Volo.Abp.Application.Dtos;
using System;

namespace HC.Documents;

public abstract class GetDocumentsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? No { get; set; }

    public string? Title { get; set; }

    public string? Type { get; set; }

    public string? UrgencyLevel { get; set; }

    public string? SecrecyLevel { get; set; }

    public string? CurrentStatus { get; set; }

    public DateTime? CompletedTimeMin { get; set; }

    public DateTime? CompletedTimeMax { get; set; }

    public Guid? FieldId { get; set; }

    public Guid? UnitId { get; set; }

    public Guid? WorkflowId { get; set; }

    public Guid? StatusId { get; set; }

    public GetDocumentsInputBase()
    {
    }
}