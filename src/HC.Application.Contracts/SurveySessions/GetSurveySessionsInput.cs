using Volo.Abp.Application.Dtos;
using System;

namespace HC.SurveySessions;

public abstract class GetSurveySessionsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PatientCode { get; set; }

    public DateTime? SurveyTimeMin { get; set; }

    public DateTime? SurveyTimeMax { get; set; }

    public DeviceType? DeviceType { get; set; }

    public string? Note { get; set; }

    public string? SessionDisplay { get; set; }

    public Guid? SurveyLocationId { get; set; }

    public GetSurveySessionsInputBase()
    {
    }
}