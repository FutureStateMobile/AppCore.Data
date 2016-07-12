using System.Collections.Generic;
using System.Configuration;
using FutureState.AppCore.Data.Tests.Helpers.Migrations;
using NUnit.Framework;
using DbConnectionProvider = FutureState.AppCore.Data.SqlServer.DbConnectionProvider;
using DbProvider = FutureState.AppCore.Data.SqlServer.DbProvider;

namespace FutureState.AppCore.Data.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        public IEnumerable<IDbProvider> DbProviders()
        {
            var testDbName = ConfigurationManager.AppSettings["databaseName"];
            var sqlServerConnection = ConfigurationManager.ConnectionStrings["databaseConnection"];

            var sqlDbConnectionProvider = new DbConnectionProvider(
                sqlServerConnection.ConnectionString,
                sqlServerConnection.ProviderName);

            IDbProvider sqlDbProvider = new DbProvider(sqlDbConnectionProvider, testDbName);
            IDbProvider sqliteDbProvider = new Sqlite.DbProvider(testDbName + ".sqlite3");

            yield return sqlDbProvider;
            yield return sqliteDbProvider;
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            foreach (var dbProvider in DbProviders())
            {
                var migrationRunner = new MigrationRunner(dbProvider);

                migrationRunner.DropDatabase();
                migrationRunner.RunAll(SystemRole.Server, new List<AppCoreMigration>
                {
                    new Migration001(),
                    new Migration002()
                });
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            // Drop the Database after all tests pass AppCore
            foreach (var dbProvider in DbProviders())
            {
                var migrationRunner = new MigrationRunner(dbProvider);
                migrationRunner.DropDatabase();
            }
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