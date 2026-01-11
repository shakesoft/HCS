using HC.SurveyLocations;
using System;
using System.Collections.Generic;
using HC.SurveySessions;

namespace HC.SurveySessions;

public abstract class SurveySessionWithNavigationPropertiesBase
{
    public SurveySession SurveySession { get; set; } = null!;
    public SurveyLocation SurveyLocation { get; set; } = null!;
}