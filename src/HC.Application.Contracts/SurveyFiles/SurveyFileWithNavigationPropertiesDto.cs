using HC.SurveySessions;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.SurveyFiles;

public abstract class SurveyFileWithNavigationPropertiesDtoBase
{
    public SurveyFileDto SurveyFile { get; set; } = null!;
    public SurveySessionDto SurveySession { get; set; } = null!;
}