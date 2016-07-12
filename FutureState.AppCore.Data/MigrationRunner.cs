using System;
using System.Collections.Generic;
using System.Linq;
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
            // Check if our database exists yet
            if (!_dbProvider.CheckIfDatabaseExists())
            {
                _dbProvider.CreateDatabase();
            }

            // Check if DatabaseVersion table is setup, if not, create it.
            if (!_dbProvider.CheckIfTableExists("DatabaseVersions"))
            {
                var database = new Database(_dbProvider.DatabaseName, _dbProvider.Dialect);

                var dbVersionTable = database.AddTable("DatabaseVersions");
                dbVersionTable.AddColumn("VersionNumber", typeof (int)).PrimaryKey().Clustered().NotNullable();
                dbVersionTable.AddColumn("MigrationDate", typeof (DateTime)).NotNullable();
                dbVersionTable.AddColumn("IsBeforeMigrationComplete", typeof (bool)).NotNullable(true);
                dbVersionTable.AddColumn("IsMigrationComplete", typeof (bool)).NotNullable(true);
                dbVersionTable.AddColumn("IsAfterMigrationComplete", typeof (bool)).NotNullable(true);

                _dbProvider.ExecuteNonQuery(database.ToString());
            }
            else
            {
                // Check if the new fields have bee added to the DatabaseVersion table yet, if not add them.
                if (!_dbProvider.CheckIfTableColumnExists("DatabaseVersions", "IsBeforeMigrationComplete"))
                {
                    var database = new Database(_dbProvider.DatabaseName, _dbProvider.Dialect);

                    var dbVersionTable = database.UpdateTable("DatabaseVersions");
                    dbVersionTable.AddColumn("IsBeforeMigrationComplete", typeof (bool)).NotNullable(true);
                    dbVersionTable.AddColumn("IsMigrationComplete", typeof (bool)).NotNullable(true);
                    dbVersionTable.AddColumn("IsAfterMigrationComplete", typeof (bool)).NotNullable(true);

                    _dbProvider.ExecuteNonQuery(database.ToString());
                }
            }
        }

        public void DropDatabase()
        {
            // drop the database in the tear down process.
            // this should only be run from the integration tests.
            if (_dbProvider.CheckIfDatabaseExists())
            {
                _dbProvider.DropDatabase();
            }
        }

        public void RunAll(SystemRole systemRole, IList<AppCoreMigration> migrations)
        {
            _systemRole = systemRole;

            CreateDatabase();

            var orderedMigrations = migrations.OrderBy(m => m.GetType().Name);

            foreach (var migration in orderedMigrations)
            {
                var databaseVersion = GetMigrationInformation(migration);

                RunBeforeMigration(migration, databaseVersion);
                RunMigration(migration, databaseVersion);
                RunAfterMigration(migration, databaseVersion);
            }
        }

        public void Run(SystemRole systemRole, AppCoreMigration migration)
        {
            _systemRole = systemRole;

            var databaseVersion = GetMigrationInformation(migration);

            RunBeforeMigration(migration, databaseVersion);
            RunMigration(migration, databaseVersion);
            RunAfterMigration(migration, databaseVersion);
        }

        private void RunBeforeMigration(AppCoreMigration migration, DatabaseVersionModel databaseVersion)
        {
            // Check Actual DatabaseVersion against the migration version
            // Don't run unless this Migrations BeforeMigration has not been run
            if (databaseVersion.IsBeforeMigrationComplete == false)
            {
                // Before Migrate

                migration.RunOrderedMigration(MigrationPoint.BeforeMigrate, _dbProvider);

                if (_systemRole == SystemRole.Server)
                {
                    migration.RunOrderedMigration(MigrationPoint.ServerBeforeMigrate, _dbProvider);
                }
                if (_systemRole == SystemRole.Client)
                {
                    migration.RunOrderedMigration(MigrationPoint.ClientBeforeMigrate, _dbProvider);
                }

                // Update the database version to show the before migration has been run
                databaseVersion.IsBeforeMigrationComplete = true;
                _dbProvider.Query<DatabaseVersionModel>()
                    .Where(dbv => dbv.VersionNumber == migration.MigrationVersion)
                    .Update(databaseVersion);
            }
        }

        private void RunMigration(AppCoreMigration migration, DatabaseVersionModel databaseVersion)
        {
            // Check Actual DatabaseVersion against the migration version
            // Don't run unless this Migrations Migration has not been run
            if (databaseVersion.IsMigrationComplete == false)
            {
                migration.RunOrderedMigration(MigrationPoint.Migrate, _dbProvider);

                if (_systemRole == SystemRole.Server)
                {
                    migration.RunOrderedMigration(MigrationPoint.ServerMigrate, _dbProvider);
                }
                if (_systemRole == SystemRole.Client)
                {
                    migration.RunOrderedMigration(MigrationPoint.ClientMigrate, _dbProvider);
                }

                // Update the database version to show the migration has been run
                databaseVersion.IsMigrationComplete = true;
                _dbProvider.Query<DatabaseVersionModel>()
                    .Where(dbv => dbv.VersionNumber == migration.MigrationVersion)
                    .Update(databaseVersion);
            }
        }

        private void RunAfterMigration(AppCoreMigration migration, DatabaseVersionModel databaseVersion)
        {
            // Check Actual DatabaseVersion against the migration version
            // Don't run unless the MigrationVersion is 1 more than DatabaseVersion
            if (databaseVersion.IsAfterMigrationComplete == false)
            {
                migration.RunOrderedMigration(MigrationPoint.AfterMigrate, _dbProvider);

                if (_systemRole == SystemRole.Server)
                {
                    migration.RunOrderedMigration(MigrationPoint.ServerAfterMigrate, _dbProvider);
                }
                if (_systemRole == SystemRole.Client)
                {
                    migration.RunOrderedMigration(MigrationPoint.ClientAfterMigrate, _dbProvider);
                }

                // Update the database version to show the after migration has been run
                databaseVersion.IsAfterMigrationComplete = true;
                _dbProvider.Query<DatabaseVersionModel>()
                    .Where(dbv => dbv.VersionNumber == migration.MigrationVersion)
                    .Update(databaseVersion);
            }
        }

        private DatabaseVersionModel GetMigrationInformation(AppCoreMigration migration)
        {
            var databaseVersion = _dbProvider.Query<DatabaseVersionModel>()
                .Where(dbv => dbv.VersionNumber == migration.MigrationVersion)
                .OrderBy(v => v.VersionNumber, OrderDirection.Descending)
                .Select().FirstOrDefault();

            if (databaseVersion == null)
            {
                databaseVersion = new DatabaseVersionModel
                {
                    VersionNumber = migration.MigrationVersion,
                    MigrationDate = DateTime.UtcNow
                };
                _dbProvider.Create(databaseVersion);
            }

            return databaseVersion;
        }
    }
}