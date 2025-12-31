using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.ProjectTaskDocuments;

public abstract class ProjectTaskDocumentUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    [StringLength(ProjectTaskDocumentConsts.DocumentPurposeMaxLength)]
    public string DocumentPurpose { get; set; }

    public Guid ProjectTaskId { get; set; }

    public Guid DocumentId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}