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
}