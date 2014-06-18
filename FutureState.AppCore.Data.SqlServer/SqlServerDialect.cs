namespace FutureState.AppCore.Data.SqlServer
{
    public class SqlServerDialect : IDialect
    {
        public string UseDatabase
        {
            get { return "USE [{0}]"; }
        }

        public string CreateTable
        {
            get { return "CREATE TABLE [{0}] ({1});"; }
        }

        public string UpdateTable
        {
            get { return "ALTER TABLE [{0}] ADD {1};"; }
        }

        public string CreateColumn
        {
            get { return "[{0}] {1} {2}"; }
        }

        public string CheckDatabaseExists
        {
            get { return "SELECT COUNT(*) AS IsExists FROM sys.databases WHERE Name = '{0}'"; }
        }

        public string CheckTableExists
        {
            get { return "SELECT COUNT(*) AS IsExists FROM dbo.sysobjects WHERE id = object_id('[dbo].[{0}]')"; }
        }

        public string CheckTableColumnExists
        {
            get { return "SELECT COUNT(*) AS IsExists FROM sys.columns WHERE [name] = '{1}' AND [object_id] = object_id('[dbo].[{0}]')"; }
        }

        public string CreateDatabase
        {
            get
            {
                return
                    "CREATE DATABASE [{0}] ALTER DATABASE [{0}] MODIFY FILE ( NAME = N'{0}' , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) ALTER DATABASE [{0}] MODIFY FILE ( NAME = N'{0}_log' , MAXSIZE = 1024GB , FILEGROWTH = 10%)";
            }
        }

        public string DropDatabase
        {
            get { return "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{0}];"; }
        }

        public string InsertInto
        {
            get { return "INSERT INTO [{0}] ({1}) VALUES ({2})"; }
        }

        public string SelectFrom
        {
            get { return "SELECT {0}.* FROM {0} {1}"; }
        }

        public string SelectCountFrom
        {
            get { return "SELECT COUNT(*) FROM {0} {1}"; }
        }

        public string DeleteFrom
        {
            get { return "DELETE FROM {0} {1}"; }
        }

        public string Update
        {
            get { return "UPDATE [{0}] SET {1} {2}"; }
        }

        public string Where
        {
            get { return "WHERE {0}"; }
        }

        public string JoinFields
        {
            get { return "[{0}], [{1}]"; }
        }

        public string JoinParameters
        {
            get { return "@{0}, @{1}"; }
        }

        public string InnerJoin
        {
            get { return "INNER JOIN {0} ON {0}.{1}Id = {2}.Id INNER JOIN {3} ON {0}.{4}Id = {3}.Id"; }
        }

        public string SkipTake
        {
            get { return "OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY"; }
        }

        // Constraints
        public string PrimaryKeyConstraint
        {
            get { return "PRIMARY KEY NONCLUSTERED"; }
        }

        public string ForeignKeyConstraint
        {
            get { return "CONSTRAINT FK_{0}_{1} FOREIGN KEY ({1}) REFERENCES {2} ({3})"; }
        }

        public string NullableConstraint
        {
            get { return "NULL"; }
        }

        public string NotNullableConstraint
        {
            get { return "NOT NULL"; }
        }

        public string OnDeleteNoActionConstraint
        {
            get { return "ON DELETE NO ACTION"; }
        }

        public string OnUpdateNoActionConstraint
        {
            get { return "ON UPDATE NO ACTION"; }
        }

        public string UniqueConstraint
        {
            get { return "UNIQUE"; }
        }

        public string DefaultBoolConstraint
        {
            get { return "DEFAULT({0})"; }
        }

        public string DefaultIntegerConstraint
        {
            get { return "DEFAULT({0})"; }
        }

        public string DefaultStringConstraint
        {
            get { return "DEFAULT '{0}'"; }
        }

        public string CompositeKeyConstraint
        {
            get { return "CONSTRAINT PK_{0}_{1}_Composite PRIMARY KEY NONCLUSTERED ({2}, {3})"; }
        }

        public string CompositeUniqueConstraint
        {
            get { return "CONSTRAINT PK_{0}_{1}_Composite UNIQUE NONCLUSTERED ({0}, {1})"; }
        }

        // Data Types
        public string Bool
        {
            get { return "bit"; }
        }

        public string Byte
        {
            get { return "tinyint"; }
        }

        public string ByteArray
        {
            get { return "binary"; }
        }

        public string DateTime
        {
            get { return "datetime"; }
        }

        public string Decimal
        {
            get { return "money"; }
        }

        public string Double
        {
            get { return "float"; }
        }

        public string Guid
        {
            get { return "uniqueidentifier"; }
        }

        public string Integer
        {
            get { return "int"; }
        }

        public string Int64
        {
            get { return "bigint"; }
        }

        public string Int16
        {
            get { return "int"; }
        }

        public string LimitedString
        {
            get { return "nvarchar({0})"; }
        }

        public string MaxString
        {
            get { return "nvarchar(max)"; }
        }

        public string Single
        {
            get { return "real"; }
        }

        public string TimeSpan
        {
            get { return "time"; }
        }

        public string OrderBy
        {
            get { return "ORDER BY {0} {1}"; }
        }

        public string Truncate
        {
            get { return "TRUNCATE TABLE {0}"; }
        }
    }
}