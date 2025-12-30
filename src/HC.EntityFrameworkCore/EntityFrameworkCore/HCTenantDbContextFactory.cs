using Microsoft.EntityFrameworkCore;

namespace HC.EntityFrameworkCore;

public class HCTenantDbContextFactory :
    HCDbContextFactoryBase<HCTenantDbContext>
{
    public HCTenantDbContextFactory()
        : base("TenantDevelopmentTime")
    {

    }

    protected override HCTenantDbContext CreateDbContext(
        DbContextOptions<HCTenantDbContext> dbContextOptions)
    {
        return new HCTenantDbContext(dbContextOptions);
    }
}
