namespace FutureState.AppCore.Data
{
    public interface IMigration
    {
        int MigrationVersion { get; }
        IDbProvider DbProvider { set; }
        void BeforeMigrate();
        void ServerBeforeMigrate();
        void ClientBeforeMigrate();
        void Migrate();
        void ServerMigrate();
        void ClientMigrate();
        void AfterMigrate();
        void ServerAfterMigrate();
        void ClientAfterMigrate();
    }
}