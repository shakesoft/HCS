using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.SurveyCriterias;

public abstract class SurveyCriteriaDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public Guid SurveyLocationId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}