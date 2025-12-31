using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.Documents;

public abstract class DocumentCreateDtoBase
{
    [StringLength(DocumentConsts.NoMaxLength)]
    public string? No { get; set; }

    [Required]
    public string Title { get; set; } = null!;
    [StringLength(DocumentConsts.TypeMaxLength)]
    public string? Type { get; set; }

    [StringLength(DocumentConsts.UrgencyLevelMaxLength)]
    public string? UrgencyLevel { get; set; }

    [StringLength(DocumentConsts.SecrecyLevelMaxLength)]
    public string? SecrecyLevel { get; set; }

    [StringLength(DocumentConsts.CurrentStatusMaxLength)]
    public string? CurrentStatus { get; set; }

    public DateTime CompletedTime { get; set; }

    public Guid? FieldId { get; set; }

    public Guid? UnitId { get; set; }

    public Guid? WorkflowId { get; set; }

    public Guid? StatusId { get; set; }
}