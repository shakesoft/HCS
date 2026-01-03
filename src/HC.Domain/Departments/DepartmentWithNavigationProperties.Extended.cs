using Volo.Abp.Identity;
using System.Collections.Generic;

namespace HC.Departments;

public class DepartmentWithNavigationProperties : DepartmentWithNavigationPropertiesBase
{
    public List<Department> Children { get; set; } = new List<Department>();
}