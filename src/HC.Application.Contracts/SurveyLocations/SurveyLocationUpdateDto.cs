using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.SurveyLocations;

public abstract class SurveyLocationUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}