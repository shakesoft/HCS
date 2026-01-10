using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.SurveyResults;

public abstract class SurveyResultUpdateDtoBase : IHasConcurrencyStamp
{
    public int Rating { get; set; }

    public Guid SurveyCriteriaId { get; set; }

    public Guid SurveySessionId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}