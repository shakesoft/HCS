using HC.SurveyLocations;
using System;
using System.Collections.Generic;
using HC.SurveyCriterias;

namespace HC.SurveyCriterias;

public abstract class SurveyCriteriaWithNavigationPropertiesBase
{
    public SurveyCriteria SurveyCriteria { get; set; } = null!;
    public SurveyLocation SurveyLocation { get; set; } = null!;
}