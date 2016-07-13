using System;
using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public enum MigrationStep
    {
        BeforeMigrate = 0,
        Migrate = 1,
        AfterMigrate = 2,
        ClientBeforeMigrate = 3,
        ClientMigrate = 4,
        ClientAfterMigrate = 5,
        ServerBeforeMigrate = 6,
        ServerMigrate = 7,
        ServerAfterMigrate = 8
    }

    public abstract class AppCoreMigration
    {
        protected AppCoreMigration(int migrationVersion)
        {
            MigrationVersion = migrationVersion;
            Migration = new Dictionary<MigrationStep, Action<Database, IDbProvider>>();
        }

        public Dictionary<MigrationStep, Action<Database, IDbProvider>> Migration { get; }

        internal int MigrationVersion { get; }

        internal void RunOrderedMigration(MigrationStep key, IDbProvider dbProvider)
        {
            if (!Migration.ContainsKey(key))
            {
                return;
            }
            var database = new Database(dbProvider.DatabaseName, dbProvider.Dialect);
            Migration[key].Invoke(database, dbProvider);
            dbProvider.ExecuteNonQuery(database.ToString());
        }
    }
}