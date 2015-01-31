using System;
using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public class Database
    {
        public readonly string Name;
        public readonly IList<Table> Tables;
        public readonly IList<String> Indices; 
        private readonly IDialect _dialect;

        public Database(string databaseName, IDialect dialect)
        {
            Name = databaseName;
            _dialect = dialect;
            Tables = new List<Table>();
            Indices = new List<string>();
        }

        public Table AddTable(string tableName)
        {
            var table = new Table(tableName, _dialect, false);
            Tables.Add(table);
            return table;
        }

        public void AddIndex(string tableName, params string[] columnNames)
        {
            var listOfColumnNames = String.Join(",", columnNames);
            var columnNamesForIndexName = String.Join("_", columnNames);
            var indexName = String.Format("IX_{0}_{1}", tableName, columnNamesForIndexName);

            Indices.Add(String.Format(_dialect.CreateIndex, indexName, tableName, listOfColumnNames));
        }

        public Table UpdateTable(string tableName)
        {
            var table = new Table( tableName, _dialect, true );
            Tables.Add( table );
            return table;
        }
        
        public override string ToString()
        {
            var ddl = String.Join("", Tables);

            if (Indices.Count > 0)
            {
                ddl += Environment.NewLine + String.Join(" ", Indices);
            }
            
            return ddl;
        }
    }
}