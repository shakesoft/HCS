using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace HC.Data;

/* This is used if database provider does't define
 * IHCDbSchemaMigrator implementation.
 */
public class NullHCDbSchemaMigrator : IHCDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
