using System;

namespace HC.MasterDatas;

public abstract class MasterDataExcelDtoBase
{
    public string Type { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}