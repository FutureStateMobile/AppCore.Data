namespace FutureState.AppCore.Data
{
    public abstract class Migration : IMigration
    {
        /// <summary>
        ///     The pre-migration setup
        /// </summary>
        /// <remarks>This will run on ALL platforms (both client and server)</remarks>
        public virtual void BeforeMigrate()
        {
        }

        /// <summary>
        ///     The pre-migration setup
        /// </summary>
        /// <remarks>This will only run on the Server and is ignored on the client.</remarks>
        public virtual void ServerBeforeMigrate()
        {
        }

        /// <summary>
        ///     The pre-migration setup
        /// </summary>
        /// <remarks>This will only run on the Client and is ignored on the Server.</remarks>
        public virtual void ClientBeforeMigrate()
        {
        }

        /// <summary>
        ///     The main migration
        /// </summary>
        /// <remarks>This will run on ALL platforms (both client and server)</remarks>
        public virtual void Migrate()
        {
        }

        /// <summary>
        ///     The main migration
        /// </summary>
        /// <remarks>This will only run on the Server and is ignored on the client.</remarks>
        public virtual void ServerMigrate()
        {
        }

        /// <summary>
        ///     The main migration
        /// </summary>
        /// <remarks>This will only run on the Client and is ignored on the Server.</remarks>
        public virtual void ClientMigrate()
        {
        }

        /// <summary>
        ///     The post-migration cleanup, tear-down and seeding
        /// </summary>
        /// <remarks>This will run on ALL platforms (both client and server)</remarks>
        public virtual void AfterMigrate()
        {
        }

        /// <summary>
        ///     The post-migration cleanup, tear-down, and seeding
        /// </summary>
        /// <remarks>This will only run on the Server and is ignored on the client</remarks>
        public virtual void ServerAfterMigrate()
        {
        }

        /// <summary>
        ///     The post-migration cleanup, tear-down, and seeding
        /// </summary>
        /// <remarks>This will only run on the Client and is ignored on the Server</remarks>
        public virtual void ClientAfterMigrate()
        {
        }

        public int MigrationVersion { get; protected set; }

        public IDbProvider DbProvider { protected get; set; }

        protected string LoadSqlFile(string fileName)
        {
            var fullFilePath = "Migrations.Migration" + MigrationVersion.ToString().PadLeft(3, '0') + "." + fileName;
            return DbProvider.LoadSqlFile(fullFilePath);
        }

        protected void ExecuteMigrationScript(string sql)
        {
            DbProvider.ExecuteNonQuery(sql);
        }
    }
}