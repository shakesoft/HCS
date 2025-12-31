using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.DocumentWorkflowInstances;

public abstract class DocumentWorkflowInstanceUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    [StringLength(DocumentWorkflowInstanceConsts.StatusMaxLength)]
    public string Status { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime FinishedAt { get; set; }

    public Guid DocumentId { get; set; }

    public Guid WorkflowId { get; set; }

    public Guid WorkflowTemplateId { get; set; }

    public Guid CurrentStepId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}