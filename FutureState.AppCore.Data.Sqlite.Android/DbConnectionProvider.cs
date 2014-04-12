using System;
using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite.Android
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly string _connectionString;

        public DbConnectionProvider(string sqliteDatabasePath)
        {
            _connectionString = "Data Source=" + sqliteDatabasePath;
        }

        public SqliteConnection GetOpenConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            if (connection == null) throw new Exception("Could not create a database connection.");

            connection.Open();

            return connection;
        }
    }
}