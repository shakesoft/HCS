using HC.SurveyCriterias;
using HC.SurveySessions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.SurveyResults;

public abstract class SurveyResultBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual int Rating { get; set; }

    public Guid SurveyCriteriaId { get; set; }

    public Guid SurveySessionId { get; set; }

    protected SurveyResultBase()
    {
    }

    public SurveyResultBase(Guid id, Guid surveyCriteriaId, Guid surveySessionId, int rating)
    {
        Id = id;
        Rating = rating;
        SurveyCriteriaId = surveyCriteriaId;
        SurveySessionId = surveySessionId;
    }
}