using System;

namespace HC.Departments;

public abstract class DepartmentExcelDtoBase
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ParentId { get; set; }

    public int Level { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}