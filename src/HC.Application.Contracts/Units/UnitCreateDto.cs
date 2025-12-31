using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.Units;

public abstract class UnitCreateDtoBase
{
    [Required]
    [StringLength(UnitConsts.CodeMaxLength, MinimumLength = UnitConsts.CodeMinLength)]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}