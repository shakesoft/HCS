using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.MasterDatas;

public abstract class MasterDataCreateDtoBase
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
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}