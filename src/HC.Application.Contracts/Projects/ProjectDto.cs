using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.Projects;

public abstract class ProjectDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public ProjectStatus Status { get; set; }

    public Guid? OwnerDepartmentId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}