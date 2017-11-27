using System;
using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public class Database
    {
        public readonly IList<string> Indices;
        public readonly string Name;
        public readonly IList<Table> Tables;
        private readonly IDialect _dialect;

        public Database(string databaseName, IDialect dialect)
        {
            Name = databaseName;
            _dialect = dialect;
            Tables = new List<Table>();
            Indices = new List<string>();
        }

        public void AddIndex(string tableName, params string[] columnNames)
        {
            var listOfColumnNames = string.Join(",", columnNames);
            var columnNamesForIndexName = string.Join("_", columnNames);
            var indexName = $"IX_{tableName}_{columnNamesForIndexName}";

            Indices.Add(string.Format(_dialect.CreateIndex, indexName, tableName, listOfColumnNames));
        }

        public Table AddTable(string tableName)
        {
            var table = new Table(tableName, _dialect, false);
            Tables.Add(table);
            return table;
        }

        public override string ToString()
        {
            var ddl = string.Join("", Tables);

            if (Indices.Count > 0)
                ddl += Environment.NewLine + string.Join(" ", Indices);

            return ddl;
        }

        public Table UpdateTable(string tableName)
        {
            var table = new Table(tableName, _dialect, true);
            Tables.Add(table);
            return table;
        }
    }
}