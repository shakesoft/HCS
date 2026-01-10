using HC.SurveyLocations;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.SurveySessions;

public abstract class SurveySessionWithNavigationPropertiesDtoBase
{
    public SurveySessionDto SurveySession { get; set; } = null!;
    public SurveyLocationDto SurveyLocation { get; set; } = null!;
}