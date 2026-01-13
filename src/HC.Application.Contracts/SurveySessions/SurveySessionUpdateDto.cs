using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.SurveySessions;

public abstract class SurveySessionUpdateDtoBase : IHasConcurrencyStamp
{
    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PatientCode { get; set; }

    public DateTime SurveyTime { get; set; }

    public DeviceType? DeviceType { get; set; }

    public string? Note { get; set; }

    public string? SessionDisplay { get; set; }
    public Guid SurveyLocationId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}