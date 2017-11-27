namespace FutureState.AppCore.Data.Sqlite
{
    public class SqliteDialect : IDialect
    {
        public string UseDatabase => "";

        public string CreateTable => "CREATE TABLE [{0}] ({1});";

        public string UpdateTable => "ALTER TABLE [{0}] ADD {1};";

        public string CreateIndex => "CREATE INDEX [{0}] ON [{1}] ({2});";

        public string CreateColumn => "[{0}] {1} {2}";

        public string CheckDatabaseExists => "";

        public string CheckTableExists => "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{0}'";

        public string CheckTableColumnExists => "SELECT SQL FROM sqlite_master WHERE tbl_name='{0}'";

        public string CreateDatabase => "";

        public string DropDatabase => "";

        public string InsertInto => "INSERT INTO [{0}] ({1}) VALUES ({2})";

        public string CreateOrUpdate => "INSERT OR REPLACE INTO [{0}] ({1}) VALUES ({2})";

        public string SelectFrom => "SELECT [{0}].* FROM [{0}] {1}";

        public string SelectCountFrom => "SELECT COUNT([{0}].*) FROM [{0}] {1}";

        public string SelectMaxFrom => "SELECT MAX([{0}].[{2}]) FROM [{0}] {1}";

        public string SelectMinFrom => "SELECT MIN([{0}].[{2}]) FROM [{0}] {1}";

        public string SelectSumFrom => "SELECT SUM([{0}].[{2}]) FROM [{0}] {1}";

        public string DeleteFrom => "DELETE FROM [{0}] {1}";

        public string Update => "UPDATE [{0}] SET {1} {2}";

        public string SelectFromJoin => "SELECT [{0}].* FROM [{0}] {1} {2}";

        public string SelectCountFromJoin => "SELECT COUNT([0].*) FROM [{0}] {1} {2}";

        public string SelectMaxFromJoin => "SELECT MAX([0].[{3}]) FROM [{0}] {1} {2}";

        public string SelectMinFromJoin => "SELECT MIN([0].[{3}]) FROM [{0}] {1} {2}";

        public string SelectSumFromJoin => "SELECT SUM([0].[{3}]) FROM [{0}] {1} {2}";

        public string DeleteFromJoin => "DELETE FROM [{0}] {1} {2}";

        public string UpdateJoin => "UPDATE [{0}] SET {1} {2} {3}";

        public string Where => "WHERE {0}";

        public string JoinFields => "[{0}], [{1}]";

        public string JoinParameters => "@{0}, @{1}";

        public string InnerJoin => "INNER JOIN [{0}] ON {1}";

        public string LeftJoin => "LEFT OUTER JOIN [{0}] ON {1}";

        public string OldManyToManyJoin => "INNER JOIN {0} ON {0}.{1}Id = {2}.Id INNER JOIN {3} ON {0}.{4}Id = {3}.Id";

        public string ManyToManyJoin => "INNER JOIN [{2}] ON [{2}].[{3}] = [{0}].[{1}] INNER JOIN [{4}] ON [{2}].[{5}] = [{4}].[{1}]";

        public string SkipTake => "LIMIT {1} OFFSET {0}";

        // Constraints
        public string PrimaryKeyConstraint => "PRIMARY KEY";

        public string ForeignKeyConstraint => "REFERENCES {2} ({3})";

        public string NullableConstraint => "NULL";

        public string NotNullableConstraint => "NOT NULL";

        public string OnDeleteNoActionConstraint => "ON DELETE NO ACTION";

        public string OnUpdateNoActionConstraint => "ON UPDATE NO ACTION";

        public string UniqueConstraint => "UNIQUE";

        public string DefaultBoolConstraint => "DEFAULT({0})";

        public string DefaultIntegerConstraint => "DEFAULT({0})";

        public string DefaultStringConstraint => "DEFAULT '{0}'";

        public string CompositeKeyConstraint => "PRIMARY KEY ({2}, {3})";

        public string CompositeUniqueConstraint => "UNIQUE ({0}, {1})";

        public string ClusteredConstraint => "";

        public string NonClusteredConstraint => "";

        // Data Types
        public string Bool => "bit";

        public string Byte => "tinyint";

        public string ByteArray => "binary";

        public string DateTime => "datetime";

        public string Decimal => "money";

        public string Double => "float";

        public string Guid => "uniqueidentifier";

        public string Integer => "int";

        public string Int64 => "bigint";

        public string Int16 => "int";

        public string LimitedString => "nvarchar({0})";

        public string MaxString => "text";

        public string Single => "real";

        public string TimeSpan => "time";

        public string OrderBy => "ORDER BY {0} {1}";

        public string Truncate => "DELETE FROM {0}";
    }
}