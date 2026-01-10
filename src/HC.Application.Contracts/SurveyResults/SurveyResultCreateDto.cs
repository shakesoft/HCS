using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.SurveyResults;

public abstract class SurveyResultCreateDtoBase
{
    public int Rating { get; set; }

    public Guid SurveyCriteriaId { get; set; }

    public Guid SurveySessionId { get; set; }
}