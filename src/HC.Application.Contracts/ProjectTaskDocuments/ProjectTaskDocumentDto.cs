using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.ProjectTaskDocuments;

public abstract class ProjectTaskDocumentDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string DocumentPurpose { get; set; }

    public Guid ProjectTaskId { get; set; }

    public Guid DocumentId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}