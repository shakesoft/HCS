using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace HC.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class HCTenantDbContext : HCDbContextBase<HCTenantDbContext>
{
    public HCTenantDbContext(DbContextOptions<HCTenantDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.SetMultiTenancySide(MultiTenancySides.Tenant);

        base.OnModelCreating(builder);
    }
}
