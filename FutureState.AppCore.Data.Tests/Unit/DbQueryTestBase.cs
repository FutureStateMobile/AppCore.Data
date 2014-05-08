using System.Collections.Generic;
using FutureState.AppCore.Data.SqlServer;
using Mono.Data.Sqlite;
using SQLiteJournalModeEnum = Mono.Data.Sqlite.SQLiteJournalModeEnum;

namespace FutureState.AppCore.Data.Tests.Unit
{
    public class DbQueryTestBase
    {
        public IEnumerable<IDbProvider> Repositories()
        {
            var cp = new DbConnectionProvider(null, null);
            var sqlServerDbProvider = new DbProvider(cp, "foo");
            var sqliteDbProvider = new Sqlite.DbProvider("foo").WithForeignKeyConstraints()
                                                               .SyncMode(SynchronizationModes.Off)
                                                               .JournalMode(SQLiteJournalModeEnum.Off)
                                                               .PageSize(65536)
                                                               .FailIfMissing(false)
                                                               .ReadOnly(false);

            yield return sqlServerDbProvider;
            yield return sqliteDbProvider;
        }
    }
}