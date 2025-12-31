using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.DocumentWorkflowInstances;

public abstract class DocumentWorkflowInstanceCreateDtoBase
{
    [Required]
    [StringLength(DocumentWorkflowInstanceConsts.StatusMaxLength)]
    public string Status { get; set; } = "DRAFT";
    public DateTime StartedAt { get; set; }

    public DateTime FinishedAt { get; set; }

    public Guid DocumentId { get; set; }

    public Guid WorkflowId { get; set; }

    public Guid WorkflowTemplateId { get; set; }

    public Guid CurrentStepId { get; set; }
}