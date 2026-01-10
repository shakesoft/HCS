using HC.SurveyCriterias;
using HC.SurveySessions;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.SurveyResults;

public abstract class SurveyResultWithNavigationPropertiesDtoBase
{
    public SurveyResultDto SurveyResult { get; set; } = null!;
    public SurveyCriteriaDto SurveyCriteria { get; set; } = null!;
    public SurveySessionDto SurveySession { get; set; } = null!;
}