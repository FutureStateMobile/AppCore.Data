using System.Collections.Generic;
using System.Threading.Tasks;

namespace FutureState.AppCore.Data
{
    public interface IMigrationRunner
    {
        void CreateDatabase();
        void DropDatabase();
        void Run(SystemRole systemRole, AppCoreMigration migrations);
        void RunAll(SystemRole systemRole, IList<AppCoreMigration> migrations);

        Task CreateDatabaseAsync();
        Task DropDatabaseAsync();
        Task RunAsync(SystemRole systemRole, AppCoreMigration migrations);
        Task RunAllAsync(SystemRole systemRole, IList<AppCoreMigration> migrations);
    }
}