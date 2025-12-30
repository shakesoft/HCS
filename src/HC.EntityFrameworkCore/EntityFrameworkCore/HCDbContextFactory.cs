using Microsoft.EntityFrameworkCore;

namespace HC.EntityFrameworkCore;

public class HCDbContextFactory :
    HCDbContextFactoryBase<HCDbContext>
{
    protected override HCDbContext CreateDbContext(
        DbContextOptions<HCDbContext> dbContextOptions)
    {
        return new HCDbContext(dbContextOptions);
    }
}
