using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using FutureState.AppCore.Data.Helpers;
using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite
{
    public class DbProvider : SqliteDbProviderBase
    {
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly bool _enforceForeignKeys = true;
        private readonly string _sqliteDatabasePath;

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

        public override bool CheckIfDatabaseExists()
        {
            var exists = File.Exists(_sqliteDatabasePath);
            return exists;
        }

        public override void CreateDatabase()
        {
            SqliteConnection.CreateFile(_sqliteDatabasePath);
        }

        public override void DropDatabase()
        {
            File.Delete(_sqliteDatabasePath);
        }

        public override bool CheckIfTableExists(string tableName)
        {
            var count = ExecuteScalar<int>(string.Format(Dialect.CheckTableExists, tableName));
            return count > 0;
        }

        public override bool CheckIfTableColumnExists(string tableName, string columnName)
        {
            var columnSql = ExecuteScalar<string>(string.Format(Dialect.CheckTableColumnExists, tableName));
            return columnSql.Contains(string.Format("[{0}]",columnName));
        }

        private void EnableForeignKeys(IDbCommand command)
        {
            if (_enforceForeignKeys)
            {
                command.CommandText = "PRAGMA foreign_keys=ON";
                command.ExecuteNonQuery();
            }
        }

        #region ExecuteReader

        public override TResult ExecuteReader<TResult>(string commandText, Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReader(commandText, new Dictionary<string, object>(), readerMapper);
        }

        public override TResult ExecuteReader<TResult>(string commandText, IDictionary<string, object> parameters,
                                                       Func<IDbReader, TResult> readerMapper)
        {
            using (var connection = _connectionProvider.GetOpenConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    EnableForeignKeys(command);
                    command.CommandText = commandText;
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(new SqliteParameter(parameter.Key, parameter.Value ?? DBNull.Value));
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        var r = new DbReader(reader);
                        return readerMapper(r);
                    }
                }
            }
        }

        #endregion

        #region ExecuteNonQuery

        public override void ExecuteNonQuery(string commandText)
        {
            ExecuteNonQuery(commandText, new Dictionary<string, object>());
        }

        public override void ExecuteNonQuery(string commandText, IDictionary<string, object> parameters)
        {
            using (var connection = _connectionProvider.GetOpenConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    EnableForeignKeys(command);
                    command.CommandText = commandText;
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(new SqliteParameter(parameter.Key, parameter.Value ?? DBNull.Value));
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region ExecuteScalar

        public override TKey ExecuteScalar<TKey>(string commandText)
        {
            return ExecuteScalar<TKey>(commandText, new Dictionary<string, object>());
        }

        public override TKey ExecuteScalar<TKey>(string commandText, IDictionary<string, object> parameters)
        {
            using (var connection = _connectionProvider.GetOpenConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    EnableForeignKeys(command);
                    command.CommandText = commandText;
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(new SqliteParameter(parameter.Key, parameter.Value ?? DBNull.Value));
                    }

                    var result = command.ExecuteScalar();

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
        }

        #endregion
    }
}