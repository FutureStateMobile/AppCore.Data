using System;
using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public class Database
    {
        public readonly string Name;
        public readonly IList<Table> Tables;
        private readonly IDialect _dialect;

        public Database(string databaseName, IDialect dialect)
        {
            Name = databaseName;
            _dialect = dialect;
            Tables = new List<Table>();
        }

        public Table AddTable(string tableName)
        {
            var table = new Table(tableName, _dialect, false);
            Tables.Add(table);
            return table;
        }

        public Table UpdateTable(string tableName)
        {
            var table = new Table( tableName, _dialect, true );
            Tables.Add( table );
            return table;
        }

        public override string ToString()
        {
            return String.Join("", Tables);
        }
    }
}