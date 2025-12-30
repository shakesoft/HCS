using System.Threading.Tasks;

namespace HC.Data;

public interface IHCDbSchemaMigrator
{
    Task MigrateAsync();
}
