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

namespace HC.SurveySessions;

public abstract class SurveySessionBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [CanBeNull]
    public virtual string? FullName { get; set; }

    [CanBeNull]
    public virtual string? PhoneNumber { get; set; }

    [CanBeNull]
    public virtual string? PatientCode { get; set; }

    public virtual DateTime SurveyTime { get; set; }

    [CanBeNull]
    public virtual string? DeviceType { get; set; }

    [CanBeNull]
    public virtual string? Note { get; set; }

    [NotNull]
    public virtual string SessionDisplay { get; set; }

    public Guid SurveyLocationId { get; set; }

    protected SurveySessionBase()
    {
    }

    public SurveySessionBase(Guid id, Guid surveyLocationId, DateTime surveyTime, string sessionDisplay, string? fullName = null, string? phoneNumber = null, string? patientCode = null, string? deviceType = null, string? note = null)
    {
        Id = id;
        Check.NotNull(sessionDisplay, nameof(sessionDisplay));
        SurveyTime = surveyTime;
        SessionDisplay = sessionDisplay;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        PatientCode = patientCode;
        DeviceType = deviceType;
        Note = note;
        SurveyLocationId = surveyLocationId;
    }
}