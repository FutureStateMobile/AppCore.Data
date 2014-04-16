namespace FutureState.AppCore.Data
{
    public interface IDbMigration
    {
        string GenerateDDL(Database database);
    }
}