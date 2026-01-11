using System;

namespace HC.SurveySessions;

public abstract class SurveySessionExcelDtoBase
{
    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PatientCode { get; set; }

    public DateTime SurveyTime { get; set; }

    public string? DeviceType { get; set; }

    public string? Note { get; set; }

    public string SessionDisplay { get; set; } = null!;
}