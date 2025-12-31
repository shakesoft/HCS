using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.Documents;

public abstract class DocumentDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string? No { get; set; }

    public string Title { get; set; } = null!;
    public string? Type { get; set; }

    public string? UrgencyLevel { get; set; }

    public string? SecrecyLevel { get; set; }

    public string? CurrentStatus { get; set; }

    public DateTime CompletedTime { get; set; }

    public Guid? FieldId { get; set; }

    public Guid? UnitId { get; set; }

    public Guid? WorkflowId { get; set; }

    public Guid? StatusId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}