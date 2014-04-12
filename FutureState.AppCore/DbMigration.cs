namespace FutureState.AppCore.Data
{
    public class DbMigration : IDbMigration
    {
        public readonly IDialect Dialect;

        public DbMigration(IDialect dialect)
        {
            Dialect = dialect;
        }

        public string GenerateDDL(Database database)
        {
            return database.ToString();
        }
    }
}