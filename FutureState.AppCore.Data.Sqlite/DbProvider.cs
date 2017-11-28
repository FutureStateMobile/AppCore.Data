using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using FutureState.AppCore.Data.Config;
using FutureState.AppCore.Data.Extensions;
using FutureState.AppCore.Data.Helpers;
using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite
{
    public class DbProvider : SqliteDbProviderBase
    {
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly bool _enforceForeignKeys = true;
        private readonly string _sqliteDatabasePath;

        public DbProvider(string databaseName, Action<DbConfiguration> config) : base(config)
        {
            DatabaseName = databaseName;
            _sqliteDatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseName);
            _connectionProvider = new DbConnectionProvider(_sqliteDatabasePath);
        }

        public DbProvider(string databaseName, SqliteSettings settings, Action<DbConfiguration> config) : base(config)
        {
            DatabaseName = databaseName;
            _enforceForeignKeys = settings.EnforceForeignKeys;
            _sqliteDatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseName);
            _connectionProvider = new DbConnectionProvider(_sqliteDatabasePath, settings);
        }

        public DbProvider(string databaseName)
        {
            DatabaseName = databaseName;
            _sqliteDatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseName);
            _connectionProvider = new DbConnectionProvider(_sqliteDatabasePath);
        }

        public DbProvider(string databaseName, SqliteSettings settings)
        {
            DatabaseName = databaseName;
            _enforceForeignKeys = settings.EnforceForeignKeys;
            _sqliteDatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseName);
            _connectionProvider = new DbConnectionProvider(_sqliteDatabasePath, settings);
        }

        public override Task<bool> CheckIfDatabaseExistsAsync() => Task.Run(() => File.Exists(_sqliteDatabasePath));

        public override Task CreateDatabaseAsync() => Task.Run(() => SqliteConnection.CreateFile(_sqliteDatabasePath));

        public override Task DropDatabaseAsync()
        {
            File.Delete(_sqliteDatabasePath);
            return Task.FromResult(true);
        }

        public override async Task<bool> CheckIfTableExistsAsync(string tableName)
        {
            var count = await ExecuteScalarAsync<int>(string.Format(Dialect.CheckTableExists, tableName)).ConfigureAwait(false);
            return count > 0;
        }

        public override async Task<bool> CheckIfTableColumnExistsAsync(string tableName, string columnName)
        {
            var columnSql = await ExecuteScalarAsync<string>(string.Format(Dialect.CheckTableColumnExists, tableName)).ConfigureAwait(false);
            return columnSql.Contains($"[{columnName}]");
        }

        private void EnableForeignKeys(IDbCommand command)
        {
            if (_enforceForeignKeys)
            {
                command.CommandText = "PRAGMA foreign_keys=ON";
                command.ExecuteNonQuery();
            }
        }

        #region ExecuteReaderAsync

        public override async Task<TResult> ExecuteReaderAsync<TResult>(string commandText, IDictionary<string, object> parameters, Func<IDbReader, TResult> readerMapper)
        {
            using (var connection = await _connectionProvider.GetOpenConnectionAsync().ConfigureAwait(false))
            using (var command = (SqliteCommand)connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                EnableForeignKeys(command);
                command.CommandText = commandText;
                parameters.ForEach(
                    parameter =>
                        command.Parameters.Add(new SqliteParameter(parameter.Key,
                            parameter.Value ?? DBNull.Value)));

                using (var reader = await command.ExecuteReaderAsync())
                using (var r = new DbReader(reader))
                {
                    return readerMapper(r);
                }
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        public override async Task ExecuteNonQueryAsync(string commandText, IDictionary<string, object> parameters)
        {
            using (var connection = await _connectionProvider.GetOpenConnectionAsync().ConfigureAwait(false))
            using (var command = (SqliteCommand)connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                EnableForeignKeys(command);
                command.CommandText = commandText;
                parameters.ForEach(
                    parameter =>
                        command.Parameters.Add(new SqliteParameter(parameter.Key,
                            parameter.Value ?? DBNull.Value)));

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteScalarAsync

        public override async Task<TKey> ExecuteScalarAsync<TKey>(string commandText, IDictionary<string, object> parameters)
        {
            using (var connection = await _connectionProvider.GetOpenConnectionAsync().ConfigureAwait(false))
            using (var command = (SqliteCommand)connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                EnableForeignKeys(command);
                command.CommandText = commandText;
                parameters.ForEach(
                    parameter =>
                        command.Parameters.Add(new SqliteParameter(parameter.Key,
                            parameter.Value ?? DBNull.Value)));

                var result = await command.ExecuteScalarAsync().ConfigureAwait(false);
                if (typeof(TKey) == typeof(Guid))
                {
                    return (TKey)(object)new Guid((byte[])result);
                }

                if (typeof(TKey) == typeof(int))
                {
                    if (result == null)
                    {
                        return (TKey)(object)0;
                    }
                    int retVal;
                    if (!int.TryParse(result.ToString(), out retVal))
                    {
                        return (TKey)(object)0;
                    }
                    return (TKey)(object)retVal;
                }

                if (typeof(TKey) == typeof(DateTime))
                {
                    DateTime retval;
                    if (!DateTime.TryParse(result.ToString(), out retval))
                    {
                        return (TKey)(object)DateTimeHelper.MinSqlValue;
                    }
                    return (TKey)(object)retval;
                }

                return (TKey)result;
            }
        }

        public override async Task RunInTransactionAsync(Action<IDbChange> dbChange)
        {

            var transaction = new SQLiteTransactionBuilder(Dialect);
            dbChange(transaction);
            using (var connection = await _connectionProvider.GetOpenConnectionAsync().ConfigureAwait(false))
            using (var trans = connection.BeginTransaction())
            using (var command = connection.CreateCommand())
            {
                command.Transaction = trans;
                try
                {
                    command.CommandType = CommandType.Text;
                    foreach (var cmd in transaction.Commands)
                    {
                        command.CommandText = cmd.CommandText;
                        cmd.CommandParameters.ForEach(parameter =>
                        {
                            command.Parameters.Add(new SqliteParameter(parameter.Key, parameter.Value ?? DBNull.Value));
                        });

                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                    trans.Commit();
                }
                catch
                {
                    trans?.Rollback();
                    throw;
                }
            }
        }

        #endregion
    }
}