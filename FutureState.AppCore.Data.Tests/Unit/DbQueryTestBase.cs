using System.Collections.Generic;
using FutureState.AppCore.Data.SqlServer;

namespace FutureState.AppCore.Data.Tests.Unit
{
    public class DbQueryTestBase
    {
        public IEnumerable<IDbProvider> Repositories()
        {
            var cp = new DbConnectionProvider(null, null);
            var sqlServerDbProvider = new DbProvider(cp, "foo");
            var sqliteDbProvider = new Sqlite.DbProvider("foo").WithForeignKeyConstraints();

            yield return sqlServerDbProvider;
            yield return sqliteDbProvider;
        }
    }
}