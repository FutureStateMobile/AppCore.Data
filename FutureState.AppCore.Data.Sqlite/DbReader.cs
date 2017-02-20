using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite
{
    public class DbReader : IDbReader
    {
        private readonly SqliteDataReader _reader;
        private Dictionary<string, int> _hashOfNames;

        public DbReader(SqliteDataReader reader)
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

        public object this[int index]
        {
            get { return _reader[index]; }
        }

        public object this[string name]
        {
            get { return HasName(name) ? _reader[name] : null; }
        }

        public int Depth
        {
            get { return _reader.Depth; }
        }

        public bool IsClosed
        {
            get { return _reader.IsClosed; }
        }

        public bool IsDbNull ( string name )
        {
            return IsDbNull(_reader.GetOrdinal(name));
        }

        public bool IsDbNull(int ordinal)
        {
            return _reader.IsDBNull(ordinal);
        }

        public int RecordsAffected
        {
            get { return _reader.RecordsAffected; }
        }

        private bool HasName(string name)
        {
            if (_hashOfNames == null)
            {
                _hashOfNames = new Dictionary<string, int>();
                for (var i = 0; i < _reader.FieldCount; i++)
                {
                    _hashOfNames.Add(_reader.GetName(i), i);
                }
            }
            return _hashOfNames.ContainsKey(name);
        }
    }
}