using System.Collections.Generic;
using FutureState.AppCore.Data.Tests.Helpers;
using FutureState.AppCore.Data.Tests.Helpers.Migrations;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Integration
{
    [SetUpFixture]
    public class DbProviderTestSetup : DbProviderTestBase
    {
        [SetUp]
        public void Setup()
        {
            foreach (var repo in DbProviders())
            {
                RunMigrations(repo);
            }
        }

        private static void RunMigrations(IDbProvider dbProvider)
        {
            var migrationRunner = new MigrationRunner(dbProvider);

            migrationRunner.DropDatabase();
            migrationRunner.RunAll(SystemRole.Server, new List<IMigration> {new Migration001()});

            // Setup Dummy Database
            SeedData.SetupFixtureDataInDatabase(dbProvider);
        }

        [TearDown]
        public void TearDown()
        {
            // Drop the Database after all tests pass AppCore
            //_migrationRunner.DropDatabase();
        }
    }
}