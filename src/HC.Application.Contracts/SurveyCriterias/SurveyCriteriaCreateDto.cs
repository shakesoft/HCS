using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.SurveyCriterias;

public abstract class SurveyCriteriaCreateDtoBase
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
}