using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace HC.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class HCDbContext : HCDbContextBase<HCDbContext>
{
    public HCDbContext(DbContextOptions<HCDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.SetMultiTenancySide(MultiTenancySides.Both);

        base.OnModelCreating(builder);
    }
}
