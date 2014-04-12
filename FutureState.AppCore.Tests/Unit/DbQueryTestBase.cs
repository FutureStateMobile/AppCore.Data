using System.Collections.Generic;
using FutureState.AppCore.Data;

namespace FutureState.AppCore.Tests.Unit
{
    public class DbQueryTestBase
    {
        public IEnumerable<IDbProvider> Repositories()
        {
            var cp = new Data.SqlServer.DbConnectionProvider( null, null );
            var sqlServerDbProvider = new Data.SqlServer.DbProvider( cp, "foo" );
            var sqliteDbProvider = new Data.Sqlite.Windows.DbProvider("foo");

            yield return sqlServerDbProvider;
            yield return sqliteDbProvider;
        }
    }
}