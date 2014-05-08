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
                    JournalMode = GetJournalMode(sqliteSettings.JournalMode),
                    PageSize = sqliteSettings.PageSize,
                    DefaultTimeout = (int) sqliteSettings.DefaultTimeout.TotalMilliseconds,
                    SyncMode = GetSyncMode(sqliteSettings.SyncMode),
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

        private static Mono.Data.Sqlite.SynchronizationModes GetSyncMode(SynchronizationModes syncMode)
        {
            switch (syncMode)
            {
                case SynchronizationModes.Full:
                    return Mono.Data.Sqlite.SynchronizationModes.Full;
                case SynchronizationModes.Normal:
                    return Mono.Data.Sqlite.SynchronizationModes.Normal;
                case SynchronizationModes.Off:
                    return Mono.Data.Sqlite.SynchronizationModes.Off;
                default:
                    return Mono.Data.Sqlite.SynchronizationModes.Normal;
            }
        }

        private static Mono.Data.Sqlite.SQLiteJournalModeEnum GetJournalMode(SQLiteJournalModeEnum journalMode)
        {
            switch (journalMode)
            {
                case SQLiteJournalModeEnum.Delete:
                    return Mono.Data.Sqlite.SQLiteJournalModeEnum.Delete;
                case SQLiteJournalModeEnum.Off:
                    return Mono.Data.Sqlite.SQLiteJournalModeEnum.Off;
                case SQLiteJournalModeEnum.Persist:
                    return Mono.Data.Sqlite.SQLiteJournalModeEnum.Persist;
                default:
                    return Mono.Data.Sqlite.SQLiteJournalModeEnum.Delete;
            }
        }
    }
}