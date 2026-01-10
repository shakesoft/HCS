using HC.SurveyLocations;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.SurveyCriterias;

public abstract class SurveyCriteriaBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string Code { get; set; }

    [NotNull]
    public virtual string Name { get; set; }

    [NotNull]
    public virtual string Image { get; set; }

    public virtual int DisplayOrder { get; set; }

    public virtual bool IsActive { get; set; }

    public Guid SurveyLocationId { get; set; }

    protected SurveyCriteriaBase()
    {
    }

    public SurveyCriteriaBase(Guid id, Guid surveyLocationId, string code, string name, string image, int displayOrder, bool isActive)
    {
        Id = id;
        Check.NotNull(code, nameof(code));
        Check.NotNull(name, nameof(name));
        Check.NotNull(image, nameof(image));
        Code = code;
        Name = name;
        Image = image;
        DisplayOrder = displayOrder;
        IsActive = isActive;
        SurveyLocationId = surveyLocationId;
    }
}