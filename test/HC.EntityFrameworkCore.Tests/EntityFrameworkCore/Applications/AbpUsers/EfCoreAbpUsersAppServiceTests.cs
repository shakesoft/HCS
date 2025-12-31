using HC.AbpUsers;
using Xunit;
using HC.EntityFrameworkCore;

namespace HC.AbpUsers;

public class EfCoreAbpUsersAppServiceTests : AbpUsersAppServiceTests<HCEntityFrameworkCoreTestModule>
{
}