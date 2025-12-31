using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.ProjectTaskDocuments;

public abstract class ProjectTaskDocumentCreateDtoBase
{
    [Required]
    [StringLength(ProjectTaskDocumentConsts.DocumentPurposeMaxLength)]
    public string DocumentPurpose { get; set; } = "REPORT";
    public Guid ProjectTaskId { get; set; }

    public Guid DocumentId { get; set; }
}