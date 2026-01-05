using System.Collections.Generic;

namespace HC.Departments;

public class DepartmentWithNavigationPropertiesDto : DepartmentWithNavigationPropertiesDtoBase
{
    public List<DepartmentDto> Children { get; set; } = new List<DepartmentDto>();
}