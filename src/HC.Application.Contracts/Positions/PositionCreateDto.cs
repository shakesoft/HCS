using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.Positions;

public abstract class PositionCreateDtoBase
{
    [Required]
    [StringLength(PositionConsts.CodeMaxLength, MinimumLength = PositionConsts.CodeMinLength)]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    [Range(PositionConsts.SignOrderMinLength, PositionConsts.SignOrderMaxLength)]
    public int SignOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}