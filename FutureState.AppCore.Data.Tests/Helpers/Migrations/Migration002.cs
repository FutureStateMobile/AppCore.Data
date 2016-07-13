using System;

namespace FutureState.AppCore.Data.Tests.Helpers.Migrations
{
    public class Migration002 : AppCoreMigration
    {
        public Migration002():base(2)
        {
            Migration[MigrationStep.Migrate] = (database, dbProvider) =>
            {
                var gooseTable = database.UpdateTable("Geese");
                gooseTable.AddColumn("BirthDate", typeof (DateTime)).Nullable();
                gooseTable.AddColumn("IsDead", typeof (bool)).NotNullable(false);
                database.AddIndex("Geese", "BirthDate");
            };
        }
    }
}