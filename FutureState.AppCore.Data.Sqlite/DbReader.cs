using System.Collections.Generic;
using System.Data.Common;

namespace FutureState.AppCore.Data.Sqlite
{
    public class DbReader : IDbReader
    {
        private readonly DbDataReader _reader;
        private Dictionary<string, int> _hashOfNames;

        public DbReader(DbDataReader reader)
        {
            _reader = reader;
        }

        public void Close()
        {
            _reader.Close();
        }

        public bool NextResult()
        {
            return _reader.NextResult();
        }

        public bool Read()
        {
            return _reader.Read();
        }

        public object this[int index] => _reader[index];

        public object this[string name] => HasName(name) ? _reader[name] : null;

        public int Depth => _reader.Depth;

        public bool IsClosed => _reader.IsClosed;

        public bool IsDbNull ( string name )
        {
            return IsDbNull(_reader.GetOrdinal(name));
        }

        public bool IsDbNull(int ordinal)
        {
            return _reader.IsDBNull(ordinal);
        }

        public int RecordsAffected => _reader.RecordsAffected;

        private bool HasName(string name)
        {
            if (_hashOfNames != null) return _hashOfNames.ContainsKey(name);
            _hashOfNames = new Dictionary<string, int>();
            for (var i = 0; i < _reader.FieldCount; i++)
            {
                _hashOfNames.Add(_reader.GetName(i), i);
            }
            return _hashOfNames.ContainsKey(name);
        }
    }
}