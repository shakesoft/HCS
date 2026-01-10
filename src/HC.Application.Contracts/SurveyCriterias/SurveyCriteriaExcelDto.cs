using System;

namespace HC.SurveyCriterias;

public abstract class SurveyCriteriaExcelDtoBase
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }
}