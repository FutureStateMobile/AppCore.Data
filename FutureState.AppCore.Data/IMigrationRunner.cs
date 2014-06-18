using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IMigrationRunner
    {
        void CreateDatabase();
        void DropDatabase();
        void Run ( SystemRole systemRole, IMigration migrations );
        void RunAll ( SystemRole systemRole, IList<IMigration> migrations );
    }
}