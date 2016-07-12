using System;
using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public abstract class AppCoreMigration
    {
        protected AppCoreMigration(int migrationVersion)
        {
            MigrationVersion = migrationVersion;
            OrderedMigrations = new Dictionary<string, Action<Database, IDbProvider>>();
        }

        /// <summary>
        ///     The pre-migration setup
        /// </summary>
        /// <remarks>This will run on ALL platforms (both client and server)</remarks>
        public void BeforeMigrate(Action<Database, IDbProvider> action)
        {
            OrderedMigrations.Add(nameof(BeforeMigrate), action);
        }

        /// <summary>
        ///     The pre-migration setup
        /// </summary>
        /// <remarks>This will only run on the Server and is ignored on the client.</remarks>
        public void ServerBeforeMigrate(Action<Database, IDbProvider> action)
        {
            OrderedMigrations.Add(nameof(ServerBeforeMigrate), action);
        }

        /// <summary>
        ///     The pre-migration setup
        /// </summary>
        /// <remarks>This will only run on the Client and is ignored on the Server.</remarks>
        public void ClientBeforeMigrate(Action<Database, IDbProvider> action)
        {
            OrderedMigrations.Add(nameof(ClientBeforeMigrate), action);
        }

        /// <summary>
        ///     The main migration
        /// </summary>
        /// <remarks>This will run on ALL platforms (both client and server)</remarks>
        public void Migrate(Action<Database, IDbProvider> action)
        {
            OrderedMigrations.Add(nameof(Migrate), action);
        }

        /// <summary>
        ///     The main migration
        /// </summary>
        /// <remarks>This will only run on the Server and is ignored on the client.</remarks>
        public void ServerMigrate(Action<Database, IDbProvider> action)
        {
            OrderedMigrations.Add(nameof(ServerMigrate), action);
        }

        /// <summary>
        ///     The main migration
        /// </summary>
        /// <remarks>This will only run on the Client and is ignored on the Server.</remarks>
        public void ClientMigrate(Action<Database, IDbProvider> action)
        {
            OrderedMigrations.Add(nameof(ClientMigrate), action);
        }

        /// <summary>
        ///     The post-migration cleanup, tear-down and seeding
        /// </summary>
        /// <remarks>This will run on ALL platforms (both client and server)</remarks>
        public void AfterMigrate(Action<Database, IDbProvider> action)
        {
            OrderedMigrations.Add(nameof(AfterMigrate), action);
        }

        /// <summary>
        ///     The post-migration cleanup, tear-down, and seeding
        /// </summary>
        /// <remarks>This will only run on the Server and is ignored on the client</remarks>
        public void ServerAfterMigrate(Action<Database, IDbProvider> action)
        {
            OrderedMigrations.Add(nameof(ServerAfterMigrate), action);
        }

        /// <summary>
        ///     The post-migration cleanup, tear-down, and seeding
        /// </summary>
        /// <remarks>This will only run on the Client and is ignored on the Server</remarks>
        public void ClientAfterMigrate(Action<Database, IDbProvider> action)
        {
            OrderedMigrations.Add(nameof(ClientAfterMigrate), action);
        }

        internal void RunOrderedMigration(string key, IDbProvider dbProvider)
        {
            if (!OrderedMigrations.ContainsKey(key))
            {
                return;
            }
            var database = new Database(dbProvider.DatabaseName, dbProvider.Dialect);
            OrderedMigrations[key].Invoke(database, dbProvider);
            dbProvider.ExecuteNonQuery(database.ToString());
        }

        internal Dictionary<string, Action<Database, IDbProvider>> OrderedMigrations { get; }

        internal int MigrationVersion { get; }
    }
}