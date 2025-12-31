using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.Positions;

public abstract class PositionUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    [StringLength(PositionConsts.CodeMaxLength, MinimumLength = PositionConsts.CodeMinLength)]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    [Range(PositionConsts.SignOrderMinLength, PositionConsts.SignOrderMaxLength)]
    public int SignOrder { get; set; }

    public bool IsActive { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}