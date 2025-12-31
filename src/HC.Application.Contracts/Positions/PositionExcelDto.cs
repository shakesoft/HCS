using System;

namespace HC.Positions;

public abstract class PositionExcelDtoBase
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int SignOrder { get; set; }

    public bool IsActive { get; set; }
}