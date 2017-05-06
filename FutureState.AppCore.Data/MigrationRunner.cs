using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutureState.AppCore.Data.Models;

namespace FutureState.AppCore.Data
{
    public class MigrationRunner : IMigrationRunner
    {
        private readonly IDbProvider _dbProvider;
        private SystemRole _systemRole;

        public MigrationRunner(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public void CreateDatabase()
        {
            CreateDatabaseAsync().Wait();
        }

        public void DropDatabase()
        {
            DropDatabaseAsync().Wait();
        }

        public void Run(SystemRole systemRole, AppCoreMigration migrations)
        {
            RunAsync(systemRole, migrations).Wait();
        }

        public void RunAll(SystemRole systemRole, IList<AppCoreMigration> migrations)
        {
            RunAllAsync(systemRole, migrations).Wait();
        }

        public async Task CreateDatabaseAsync()
        {
            // Check if our database exists yet
            if (! await _dbProvider.CheckIfDatabaseExistsAsync().ConfigureAwait(false))
            {
                await _dbProvider.CreateDatabaseAsync().ConfigureAwait(false);
            }

            // Check if DatabaseVersion table is setup, if not, create it.
            if (!await _dbProvider.CheckIfTableExistsAsync("DatabaseVersions").ConfigureAwait(false))
            {
                var database = new Database(_dbProvider.DatabaseName, _dbProvider.Dialect);

                var dbVersionTable = database.AddTable("DatabaseVersions");
                dbVersionTable.AddColumn("VersionNumber", typeof (int)).PrimaryKey().Clustered().NotNullable();
                dbVersionTable.AddColumn("MigrationDate", typeof (DateTime)).NotNullable();
                dbVersionTable.AddColumn("IsBeforeMigrationComplete", typeof (bool)).NotNullable(true);
                dbVersionTable.AddColumn("IsMigrationComplete", typeof (bool)).NotNullable(true);
                dbVersionTable.AddColumn("IsAfterMigrationComplete", typeof (bool)).NotNullable(true);

                await _dbProvider.ExecuteNonQueryAsync(database.ToString()).ConfigureAwait(false);
            }
            else
            {
                // Check if the new fields have bee added to the DatabaseVersion table yet, if not add them.
                if (!await _dbProvider.CheckIfTableColumnExistsAsync("DatabaseVersions", "IsBeforeMigrationComplete").ConfigureAwait(false))
                {
                    var database = new Database(_dbProvider.DatabaseName, _dbProvider.Dialect);

                    var dbVersionTable = database.UpdateTable("DatabaseVersions");
                    dbVersionTable.AddColumn("IsBeforeMigrationComplete", typeof (bool)).NotNullable(true);
                    dbVersionTable.AddColumn("IsMigrationComplete", typeof (bool)).NotNullable(true);
                    dbVersionTable.AddColumn("IsAfterMigrationComplete", typeof (bool)).NotNullable(true);

                    await _dbProvider.ExecuteNonQueryAsync(database.ToString()).ConfigureAwait(false);
                }
            }
        }

        public async Task DropDatabaseAsync()
        {
            // drop the database in the tear down process.
            // this should only be run from the integration tests.
            if (await _dbProvider.CheckIfDatabaseExistsAsync().ConfigureAwait(false))
            {
                await _dbProvider.DropDatabaseAsync().ConfigureAwait(false);
            }
        }

        public async Task RunAllAsync(SystemRole systemRole, IList<AppCoreMigration> migrations)
        {
            _systemRole = systemRole;

            await CreateDatabaseAsync().ConfigureAwait(false);

            var orderedMigrations = migrations.OrderBy(m => m.GetType().Name);

            foreach (var migration in orderedMigrations)
            {
                var databaseVersion = await GetMigrationInformationAsync(migration).ConfigureAwait(false);

                await RunBeforeMigrationAsync(migration,  databaseVersion).ConfigureAwait(false);
                await RunMigrationAsync(migration,  databaseVersion).ConfigureAwait(false);
                await RunAfterMigrationAsync(migration,  databaseVersion).ConfigureAwait(false);
            }
        }

        public async Task RunAsync(SystemRole systemRole, AppCoreMigration migration)
        {
            _systemRole = systemRole;

            var databaseVersion = await GetMigrationInformationAsync(migration).ConfigureAwait(false);

            await RunBeforeMigrationAsync(migration,  databaseVersion).ConfigureAwait(false);
            await RunMigrationAsync(migration,  databaseVersion).ConfigureAwait(false);
            await RunAfterMigrationAsync(migration,  databaseVersion).ConfigureAwait(false);
        }

        private async Task RunBeforeMigrationAsync(AppCoreMigration migration, DatabaseVersionModel databaseVersion)
        {
            // Check Actual DatabaseVersion against the migration version
            // Don't run unless this Migrations BeforeMigration has not been run
            if (databaseVersion.IsBeforeMigrationComplete == false)
            {
                // Before Migrate

                await migration.RunOrderedMigrationAsync(MigrationStep.BeforeMigrate, _dbProvider).ConfigureAwait(false);

                if (_systemRole == SystemRole.Server)
                {
                    await migration.RunOrderedMigrationAsync(MigrationStep.ServerBeforeMigrate, _dbProvider).ConfigureAwait(false);
                }
                if (_systemRole == SystemRole.Client)
                {
                    await migration.RunOrderedMigrationAsync(MigrationStep.ClientBeforeMigrate, _dbProvider).ConfigureAwait(false);
                }

                // UpdateAsync the database version to show the before migration has been run
                databaseVersion.IsBeforeMigrationComplete = true;
                await _dbProvider.Query<DatabaseVersionModel>()
                    .Where(dbv => dbv.VersionNumber == migration.MigrationVersion)
                    .UpdateAsync(databaseVersion)
                    .ConfigureAwait(false);
            }
        }

        private async Task RunMigrationAsync(AppCoreMigration migration, DatabaseVersionModel databaseVersion)
        {
            // Check Actual DatabaseVersion against the migration version
            // Don't run unless this Migrations Migration has not been run
            if (databaseVersion.IsMigrationComplete == false)
            {
                await migration.RunOrderedMigrationAsync(MigrationStep.Migrate, _dbProvider).ConfigureAwait(false);

                if (_systemRole == SystemRole.Server)
                {
                    await migration.RunOrderedMigrationAsync(MigrationStep.ServerMigrate, _dbProvider).ConfigureAwait(false);
                }
                if (_systemRole == SystemRole.Client)
                {
                    await migration.RunOrderedMigrationAsync(MigrationStep.ClientMigrate, _dbProvider).ConfigureAwait(false);
                }

                // UpdateAsync the database version to show the migration has been run
                databaseVersion.IsMigrationComplete = true;
                await _dbProvider.Query<DatabaseVersionModel>()
                    .Where(dbv => dbv.VersionNumber == migration.MigrationVersion)
                    .UpdateAsync(databaseVersion)
                    .ConfigureAwait(false);
            }

        }

        private async Task RunAfterMigrationAsync(AppCoreMigration migration, DatabaseVersionModel databaseVersion)
        {
            // Check Actual DatabaseVersion against the migration version
            // Don't run unless the MigrationVersion is 1 more than DatabaseVersion
            if (databaseVersion.IsAfterMigrationComplete == false)
            {
                await migration.RunOrderedMigrationAsync(MigrationStep.AfterMigrate, _dbProvider).ConfigureAwait(false);

                if (_systemRole == SystemRole.Server)
                {
                    await migration.RunOrderedMigrationAsync(MigrationStep.ServerAfterMigrate, _dbProvider).ConfigureAwait(false);
                }
                if (_systemRole == SystemRole.Client)
                {
                    await migration.RunOrderedMigrationAsync(MigrationStep.ClientAfterMigrate, _dbProvider).ConfigureAwait(false);
                }

                // UpdateAsync the database version to show the after migration has been run
                databaseVersion.IsAfterMigrationComplete = true;
                await _dbProvider.Query<DatabaseVersionModel>()
                    .Where(dbv => dbv.VersionNumber == migration.MigrationVersion)
                    .UpdateAsync(databaseVersion)
                    .ConfigureAwait(false);
            }
        }

        private async Task<DatabaseVersionModel> GetMigrationInformationAsync(AppCoreMigration migration)
        {
            var databaseVersions = await _dbProvider.Query<DatabaseVersionModel>()
                .Where(dbv => dbv.VersionNumber == migration.MigrationVersion)
                .OrderBy(v => v.VersionNumber, OrderDirection.Descending)
                .SelectAsync()
                .ConfigureAwait(false);
            var databaseVersion = databaseVersions.FirstOrDefault();

            if (databaseVersion == null)
            {
                databaseVersion = new DatabaseVersionModel
                {
                    VersionNumber = migration.MigrationVersion,
                    MigrationDate = DateTime.UtcNow
                };
                await _dbProvider.CreateAsync(databaseVersion).ConfigureAwait(false);
            }

            return databaseVersion;
        }
    }
}