using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.SurveySessions;

public abstract class SurveySessionCreateDtoBase
{
    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PatientCode { get; set; }

    public DateTime SurveyTime { get; set; }

    public string? DeviceType { get; set; }

    public string? Note { get; set; }

    [Required]
    public string SessionDisplay { get; set; } = null!;
    public Guid SurveyLocationId { get; set; }
}