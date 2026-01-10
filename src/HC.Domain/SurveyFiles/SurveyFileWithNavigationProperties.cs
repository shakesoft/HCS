using HC.SurveySessions;
using System;
using System.Collections.Generic;
using HC.SurveyFiles;

namespace HC.SurveyFiles;

public abstract class SurveyFileWithNavigationPropertiesBase
{
    public SurveyFile SurveyFile { get; set; } = null!;
    public SurveySession SurveySession { get; set; } = null!;
}