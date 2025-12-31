using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.Units;

public abstract class UnitUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    [StringLength(UnitConsts.CodeMaxLength, MinimumLength = UnitConsts.CodeMinLength)]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}