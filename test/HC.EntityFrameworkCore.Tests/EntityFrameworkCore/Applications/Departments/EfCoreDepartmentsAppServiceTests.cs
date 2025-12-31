using HC.Departments;
using Xunit;
using HC.EntityFrameworkCore;

namespace HC.Departments;

public class EfCoreDepartmentsAppServiceTests : DepartmentsAppServiceTests<HCEntityFrameworkCoreTestModule>
{
}