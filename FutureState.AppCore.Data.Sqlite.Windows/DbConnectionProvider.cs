using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite.Windows
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly string _connectionString;

        public DbConnectionProvider(string sqlFile)
        {
            _connectionString = string.Format("Data Source={0};", sqlFile);
        }

        public SqliteConnection GetOpenConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            if (connection == null) throw new SqliteException("Could not create a database connection.");

            connection.Open();

            return connection;
        }
    }
}