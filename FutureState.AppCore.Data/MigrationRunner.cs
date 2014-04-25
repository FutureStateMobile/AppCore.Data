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
                var migration = new DbMigration(_dbProvider.Dialect);
                var database = new Database(_dbProvider.DatabaseName, _dbProvider.Dialect);

                var dbVersionTable = database.AddTable("DatabaseVersions");
                dbVersionTable.AddColumn("VersionNumber", typeof (int)).PrimaryKey().NotNullable();
                dbVersionTable.AddColumn("MigrationDate", typeof (DateTime)).NotNullable();

                _dbProvider.ExecuteNonQuery(migration.GenerateDDL(database));
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

        public void RunAll(SystemRole systemRole, IList<IMigration> migrations)
        {
            _systemRole = systemRole;

            CreateDatabase();

            var orderedMigrations = migrations.OrderBy(m => m.GetType().Name);

            foreach (var migration in orderedMigrations)
            {
                Run(migration);
            }
        }

        public void Run(IMigration migration)
        {
            // Check Actual DatabaseVersion against the migration version
            // Don't run unless the MigrationVersion is 1 more than DatabaseVersion
            var databaseVersion = _dbProvider.Query<DatabaseVersionModel>().Select().FirstOrDefault();

            if (databaseVersion == null || databaseVersion.VersionNumber + 1 == migration.MigrationVersion)
            {
                migration.DbProvider = _dbProvider;

                // Before Migrate
                migration.BeforeMigrate();
                if (_systemRole == SystemRole.Server) migration.ServerBeforeMigrate();
                if (_systemRole == SystemRole.Client) migration.ClientBeforeMigrate();

                // Migrate                
                migration.Migrate();
                if (_systemRole == SystemRole.Server) migration.ServerMigrate();
                if (_systemRole == SystemRole.Client) migration.ClientMigrate();

                // After Migrate
                migration.AfterMigrate();
                if (_systemRole == SystemRole.Server) migration.ServerAfterMigrate();
                if (_systemRole == SystemRole.Client) migration.ClientAfterMigrate();

                // Update the database version number to this version
                _dbProvider.Create(new DatabaseVersionModel
                    {
                        VersionNumber = migration.MigrationVersion,
                        MigrationDate = DateTime.UtcNow
                    });
            }
        }
    }
}