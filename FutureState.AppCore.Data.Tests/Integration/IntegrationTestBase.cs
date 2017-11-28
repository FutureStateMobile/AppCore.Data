using System.Collections.Generic;
using System.Configuration;
using FutureState.AppCore.Data.Config;
using FutureState.AppCore.Data.Tests.Helpers.Migrations;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;
using SqlDbConnectionProvider = FutureState.AppCore.Data.SqlServer.DbConnectionProvider;
using SqlDbProvider = FutureState.AppCore.Data.SqlServer.DbProvider;
using SqliteDbProvider = FutureState.AppCore.Data.Sqlite.DbProvider;
namespace FutureState.AppCore.Data.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        public IEnumerable<IDbProvider> DbProviders()
        {
            var testDbName = ConfigurationManager.AppSettings["databaseName"];
            var sqlServerConnection = ConfigurationManager.ConnectionStrings["databaseConnection"];

            var sqlDbConnectionProvider = new SqlDbConnectionProvider(
                sqlServerConnection.ConnectionString,
                sqlServerConnection.ProviderName);

            yield return new SqlDbProvider(sqlDbConnectionProvider, testDbName,SetConfig);
            yield return new SqliteDbProvider(testDbName + ".sqlite3", SetConfig);
        }

        [SetUp]
        public void Setup()
        {
            foreach (var dbProvider in DbProviders())
            {
                var migrationRunner = new MigrationRunner(dbProvider);

                migrationRunner.DropDatabase();
                migrationRunner.RunAll(SystemRole.Server, new List<AppCoreMigration>
                {
                    new Migration001(),
                    new Migration002(),
                    new Migration003()
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

        private static void SetConfig(DbConfiguration cfg)
        {
            cfg.Configure<AutomobileModel>(opts => opts.SetPrimaryKey(a => a.Vin));
        }
    }
}