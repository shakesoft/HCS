using HC.Projects;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.Projects;

public abstract class ProjectCreateDtoBase
{
    [Required]
    [StringLength(ProjectConsts.CodeMaxLength)]
    public string Code { get; set; } = null!;
    [Required]
    [StringLength(ProjectConsts.NameMaxLength)]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public ProjectStatus Status { get; set; } = ((ProjectStatus[])Enum.GetValues(typeof(ProjectStatus)))[0];
    public Guid? OwnerDepartmentId { get; set; }
}