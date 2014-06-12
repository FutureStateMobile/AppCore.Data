using System;

namespace FutureState.AppCore.Data.Tests.Helpers.Migrations
{
    public class Migration002 : Migration
    {
        public Migration002()
        {
            MigrationVersion = 2;
        }

        public override void Migrate()
        {
            var migration = new DbMigration(DbProvider.Dialect);
            var database = new Database(DbProvider.DatabaseName, DbProvider.Dialect);

            var gooseTable = database.UpdateTable("Geese");
            gooseTable.AddColumn( "BirthDate", typeof( DateTime ) ).Nullable();
            gooseTable.AddColumn( "IsDead", typeof( bool ) ).NotNullable( false );

            DbProvider.ExecuteNonQuery(migration.GenerateDDL(database));
        }
    }
}