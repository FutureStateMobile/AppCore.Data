using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IMigrationRunner
    {
        void CreateDatabase();
        void DropDatabase();
        void RunAll(SystemRole systemRole, IList<IMigration> migrations);
        void Run(IMigration migration);
    }
}