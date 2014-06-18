using System;

namespace FutureState.AppCore.Data
{
    public interface IDialect
    {
        string UseDatabase { get; }
        string CreateTable { get; }
        string UpdateTable { get; }
        string CreateColumn { get; }
        string CheckDatabaseExists { get; }
        string CheckTableExists { get; }
        string CheckTableColumnExists { get; }
        string CreateDatabase { get; }
        string DropDatabase { get; }

        string InsertInto { get; }
        string SelectFrom { get; }
        string SelectCountFrom { get; }
        string DeleteFrom { get; }
        string Update { get; }
        string Where { get; }
        string JoinFields { get; }
        string JoinParameters { get; }
        string InnerJoin { get; }
        string SkipTake { get; }

        // Constraints
        string PrimaryKeyConstraint { get; }
        string ForeignKeyConstraint { get; }
        string NotNullableConstraint { get; }
        string NullableConstraint { get; }
        string OnDeleteNoActionConstraint { get; }
        string OnUpdateNoActionConstraint { get; }
        string UniqueConstraint { get; }
        string DefaultBoolConstraint { get; }
        string DefaultIntegerConstraint { get; }
        string DefaultStringConstraint { get; }
        string CompositeKeyConstraint { get; }
        string CompositeUniqueConstraint { get; }

        // Data Types
        string Bool { get; }
        string Byte { get; }
        string ByteArray { get; }
        string DateTime { get; }
        string Decimal { get; }
        string Double { get; }
        string Guid { get; }
        string Integer { get; }
        string Int64 { get; }
        string Int16 { get; }
        string LimitedString { get; }
        string MaxString { get; }
        string Single { get; }
        string TimeSpan { get; }
        string OrderBy { get; }
        string Truncate { get; }
    }
}