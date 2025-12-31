using System;

namespace HC.Units;

public abstract class UnitExcelDtoBase
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}