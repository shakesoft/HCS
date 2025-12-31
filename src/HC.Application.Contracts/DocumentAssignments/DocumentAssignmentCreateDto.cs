using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.DocumentAssignments;

public abstract class DocumentAssignmentCreateDtoBase
{
    [Range(DocumentAssignmentConsts.StepOrderMinLength, DocumentAssignmentConsts.StepOrderMaxLength)]
    public int StepOrder { get; set; } = 0;
    [Required]
    [StringLength(DocumentAssignmentConsts.ActionTypeMaxLength)]
    public string ActionType { get; set; } = "PROCESS";
    [Required]
    [StringLength(DocumentAssignmentConsts.StatusMaxLength)]
    public string Status { get; set; } = "PENDING";
    public DateTime AssignedAt { get; set; }

    public DateTime ProcessedAt { get; set; }

    public bool IsCurrent { get; set; } = true;
    public Guid DocumentId { get; set; }

    public Guid StepId { get; set; }

    public Guid ReceiverUserId { get; set; }
}