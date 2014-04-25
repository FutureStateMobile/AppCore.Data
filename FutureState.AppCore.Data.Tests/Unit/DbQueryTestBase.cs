using System.Collections.Generic;

namespace FutureState.AppCore.Data.Tests.Unit
{
    public class DbQueryTestBase
    {
        public IEnumerable<IDbProvider> Repositories()
        {
            var cp = new SqlServer.DbConnectionProvider( null, null );
            var sqlServerDbProvider = new SqlServer.DbProvider( cp, "foo" );
            var sqliteDbProvider = new Sqlite.DbProvider("foo").WithForeignKeyConstraints();

            yield return sqlServerDbProvider;
            yield return sqliteDbProvider;
        }
    }
}