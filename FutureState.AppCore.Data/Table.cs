using System;
using System.Collections.Generic;
using FutureState.AppCore.Data.Constraints;

namespace FutureState.AppCore.Data
{
    public class Table
    {
        public readonly IList<IConstraint> Constraints;
        public readonly string Name;
        private readonly IDialect _dialect;
        private readonly bool _updateTable;

        public Table(string name, IDialect dialect, bool updateTable)
        {
            Name = name;
            Columns = new List<Column>();
            Constraints = new List<IConstraint>();
            _dialect = dialect;
            _updateTable = updateTable;
        }

        public IList<Column> Columns { get; set; }

        public Table CompositeKey(string key1, string key2)
        {
            Constraints.Add(new CompositeKeyConstraint(_dialect, Name, key1, key2));
            return this;
        }

        public Table CompositeUnique(string key1, string key2)
        {
            Constraints.Add(new CompositeUniqueConstraint(_dialect, key1, key2));
            return this;
        }

        public Column AddColumn(string columnName, Type dataType)
        {
            var column = AddColumn(columnName, dataType, 0);
            return column;
        }

        public Column AddColumn(string columnName, Type dataType, int precision)
        {
            var column = new Column(_dialect, columnName, dataType, Name, precision);
            Columns.Add(column);
            return column;
        }

        public override string ToString()
        {
            if (Constraints.Count != 0)
            {
                var columns = string.Join(",", Columns);
                var constraints = Environment.NewLine + string.Join(",", Constraints);

                if (_updateTable)
                {
                    return Environment.NewLine +
                           string.Format( _dialect.UpdateTable, Name, string.Join( ",", new[] { columns, constraints } ) );
                }

                return Environment.NewLine +
                        string.Format( _dialect.CreateTable, Name, string.Join( ",", new[] { columns, constraints } ) );
            }

            if (_updateTable)
            {
                return Environment.NewLine + string.Format( _dialect.UpdateTable, Name, string.Join( ",", Columns ) );
            }

            return Environment.NewLine + string.Format( _dialect.CreateTable, Name, string.Join( ",", Columns ) );
        }
    }
}