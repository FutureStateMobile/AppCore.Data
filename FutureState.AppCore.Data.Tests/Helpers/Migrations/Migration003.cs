namespace FutureState.AppCore.Data.Tests.Helpers.Migrations
{
    public class Migration003 : AppCoreMigration
    {
        public Migration003() : base(3)
        {
            Migration[MigrationStep.Migrate] = (database, provider) =>
            {
                var autoTable = database.AddTable("Automobiles");
                autoTable.AddColumn("Vin", typeof(string),50).PrimaryKey().NotNullable();
                autoTable.AddColumn("VehicleType", typeof(string));
                autoTable.AddColumn("WheelCount", typeof(int));
            };
        }
    }
}