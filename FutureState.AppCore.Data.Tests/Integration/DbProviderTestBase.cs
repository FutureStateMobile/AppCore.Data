using System.Collections.Generic;
using System.Configuration;
using FutureState.AppCore.Data.Sqlite;
using DbConnectionProvider = FutureState.AppCore.Data.SqlServer.DbConnectionProvider;
using DbProvider = FutureState.AppCore.Data.SqlServer.DbProvider;

namespace FutureState.AppCore.Data.Tests.Integration
{
    public abstract class DbProviderTestBase
    {
        public IEnumerable<IDbProvider> DbProviders()
        {
            var testDbName = ConfigurationManager.AppSettings["databaseName"];
            var sqlServerConnection = ConfigurationManager.ConnectionStrings["databaseConnection"];

            var sqlDbConnectionProvider = new DbConnectionProvider(
                sqlServerConnection.ConnectionString,
                sqlServerConnection.ProviderName);

            IDbProvider sqlDbProvider = new DbProvider(sqlDbConnectionProvider, testDbName);

            IDbProvider sqliteDbProvider = new Sqlite.DbProvider(testDbName, new SqliteSettings
                {
                    CacheSize = 16777216,
                    SyncMode = SynchronizationModes.Off,
                    JournalMode = SQLiteJournalModeEnum.Off,
                    PageSize = 65536
                }).WithForeignKeyConstraints();

            yield return sqlDbProvider;
            yield return sqliteDbProvider;
        }

        protected string TraceObjectGraphInfo(IDbProvider dbProvider)
        {
            var dbProviderFriendlyName = dbProvider.GetType().ToString()
                                                   .Replace("FutureState.AppCore.Data.", "'")
                                                   .Replace(".", "' ");

            return "Tested against the " + dbProviderFriendlyName;
        }
    }
}