using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.SurveyLocations;

public abstract class SurveyLocationCreateDtoBase
{
    [Required]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}