using HC.SurveyLocations;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.SurveyCriterias;

public abstract class SurveyCriteriaWithNavigationPropertiesDtoBase
{
    public SurveyCriteriaDto SurveyCriteria { get; set; } = null!;
    public SurveyLocationDto SurveyLocation { get; set; } = null!;
}