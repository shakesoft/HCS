using HC.Projects;
using HC.Departments;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.Projects;

public abstract class ProjectBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string Code { get; set; }

    [NotNull]
    public virtual string Name { get; set; }

    [CanBeNull]
    public virtual string? Description { get; set; }

    public virtual DateTime StartDate { get; set; }

    public virtual DateTime EndDate { get; set; }

    public virtual ProjectStatus Status { get; set; }

    public Guid? OwnerDepartmentId { get; set; }

    protected ProjectBase()
    {
    }

    public ProjectBase(Guid id, Guid? ownerDepartmentId, string code, string name, DateTime startDate, DateTime endDate, ProjectStatus status, string? description = null)
    {
        Id = id;
        Check.NotNull(code, nameof(code));
        Check.Length(code, nameof(code), ProjectConsts.CodeMaxLength, 0);
        Check.NotNull(name, nameof(name));
        Check.Length(name, nameof(name), ProjectConsts.NameMaxLength, 0);
        Code = code;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        Description = description;
        OwnerDepartmentId = ownerDepartmentId;
    }
}