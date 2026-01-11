using System;

namespace HC.SurveyLocations;

public abstract class SurveyLocationExcelDtoBase
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}