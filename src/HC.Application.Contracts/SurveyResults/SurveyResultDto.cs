using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.SurveyResults;

public abstract class SurveyResultDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public int Rating { get; set; }

    public Guid SurveyCriteriaId { get; set; }

    public Guid SurveySessionId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}