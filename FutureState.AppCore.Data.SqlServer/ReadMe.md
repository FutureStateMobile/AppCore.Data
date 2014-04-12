#SQL Server Scripts.

*note: the use of SQL Scripts should only be used in corner cases where you cannot effectively extend the `FutureState.AppCore.Data.DbProviderExtensions` &/ the `FutureState.AppCore.Data.DbMigration` ORM*

When creating or updating scripts, there are a couple of locations that contain the properties.

  - FutureState.AppCore.Data.SqlServer\
    - Migrations\Migration[nnn]\
      - If there are historical migrations that contain the properties that you are changing, you have to make sure that you make that change in the ***last*** migration only.
    - Repositories\
      - [TableName]\
        - InsertInto[TableName].sql
        - SelectFrom[TableName].sql
        - Update[TableName].sql
        - etc...

-----

If your change reqires a NEW migration rather than just updating the existing one, you need to execute the migration

  - FutureState.AppCore\
    - Migrations\
      - Migration[nnn].cs


To execute the migration, Just add `ExecuteMigrationScript(LoadSqlFile("Update[TableName].sql"));`

    public override void Migrate()
    {
        ExecuteMigrationScript(LoadSqlFile("DoSomething.sql"));
    }

-----

All tables should inherit from the `ModelBase` class.

    public class SomeTableModel : ModelBase
    {
        public bool Foo { get; set; }
    }

    // will result in
    
    public class SomeTableModel : ModelBaseSystem
    {
        public bool Foo { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }

If the table will be managed by a system account, please add the `IsSystem` property directly to the class, and the table.

    public class SomeTableModel : ModelBase
    {
        public bool Foo { get; set; }
        public bool IsSystem { get; set; }
    }
        