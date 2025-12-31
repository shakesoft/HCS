using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.Projects;

public abstract class ProjectUpdateDtoBase : IHasConcurrencyStamp
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

    [Required]
    [StringLength(ProjectConsts.StatusMaxLength)]
    public string Status { get; set; }

    public Guid? OwnerDepartmentId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}