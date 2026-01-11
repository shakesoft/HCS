using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.SurveyCriterias;

public abstract class SurveyCriteriaUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Image { get; set; } = null!;
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public Guid SurveyLocationId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}