using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.MasterDatas;

public abstract class MasterDataUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    [StringLength(MasterDataConsts.TypeMaxLength, MinimumLength = MasterDataConsts.TypeMinLength)]
    public string Type { get; set; } = null!;
    [Required]
    [StringLength(MasterDataConsts.CodeMaxLength, MinimumLength = MasterDataConsts.CodeMinLength)]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    [Range(MasterDataConsts.SortOrderMinLength, MasterDataConsts.SortOrderMaxLength)]
    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}