using System;

namespace FutureState.AppCore.Data.Constraints
{
    public class CompositeKeyConstraint : IConstraint
    {
        private readonly IDialect _dialect;
        private readonly string _key1;
        private readonly string _key2;
        private readonly string[] _tables;

        public CompositeKeyConstraint(IDialect dialect, string tableName, string key1, string key2)
        {
            _dialect = dialect;
            _key1 = key1;
            _key2 = key2;
            _tables = tableName.Split(Convert.ToChar("_"));
        }

        public override string ToString()
        {
            return string.Format(_dialect.CompositeKeyConstraint, _tables[0], _tables[1], _key1, _key2);
        }
    }
}