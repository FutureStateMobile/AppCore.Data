using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly string _connectionString;

        public DbConnectionProvider(string sqlFile, SqliteSettings sqliteSettings)
        {
            var dataSource = string.Format("Data Source={0};", sqlFile);
            var sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder(dataSource)
                {
                    CacheSize = sqliteSettings.CacheSize,
                    JournalMode = sqliteSettings.JournalMode,
                    PageSize = sqliteSettings.PageSize,
                    DefaultTimeout = (int)sqliteSettings.DefaultTimeout.TotalMilliseconds,
                    SyncMode = sqliteSettings.SyncMode,
                    FailIfMissing = sqliteSettings.FailIfMissing,
                    ReadOnly = sqliteSettings.ReadOnly,
                };

            _connectionString = sqliteConnectionStringBuilder.ConnectionString;
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