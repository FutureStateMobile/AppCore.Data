using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FutureState.AppCore.Data.Extensions;
using FutureState.AppCore.Data.Helpers;

namespace FutureState.AppCore.Data.SqlServer
{
    public class DbProvider : DbProviderBase
    {
        private const string _rootSqlScriptPath = "FutureState.AppCore.Data.SqlServer.SqlScripts.";
        private static string _useStatement;
        private readonly IDbConnectionProvider _connectionProvider;
        private IDialect _dialect;

        public DbProvider(IDbConnectionProvider connectionProvider, string databaseName)
        {
            DatabaseName = databaseName;
            _connectionProvider = connectionProvider;
            _useStatement = string.Format(Dialect.UseDatabase, databaseName);
        }

        public sealed override IDialect Dialect => _dialect ?? (_dialect = new SqlServerDialect());

        public override async Task<string> LoadSqlFileAsync<TDbProvider>(string fileName)
        {
            var sqlStatement = string.Empty;

            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_rootSqlScriptPath + fileName))
            {
                if (resourceStream != null)
                {
                    sqlStatement = await new StreamReader(resourceStream).ReadToEndAsync().ConfigureAwait(false);
                }
            }

            return sqlStatement;
        }

        public override async Task<bool> CheckIfDatabaseExistsAsync()
        {
            return await ExecuteScalarAsync<int>("", string.Format(Dialect.CheckDatabaseExists, DatabaseName)).ConfigureAwait(false) == 1;
            //return ExecuteScalarAsync<int>("USE master;", string.Format(Dialect.CheckDatabaseExists, DatabaseName)) == 1;
        }

        public override Task CreateDatabaseAsync()
        {
            return ExecuteNonQueryAsync("", string.Format(Dialect.CreateDatabase, DatabaseName));
            //ExecuteNonQueryAsync("USE master; ", string.Format(Dialect.CreateDatabaseAsync, DatabaseName));
        }

        public override Task DropDatabaseAsync()
        {
            return ExecuteNonQueryAsync("", string.Format(Dialect.DropDatabase, DatabaseName));
            //ExecuteNonQueryAsync("USE master; ", string.Format(Dialect.DropDatabaseAsync, DatabaseName));
        }

        public override async Task<bool> CheckIfTableExistsAsync(string tableName)
        {
            return await ExecuteScalarAsync<int>(string.Format(Dialect.CheckTableExists, tableName)).ConfigureAwait(false) == 1;
        }

        public override async Task<bool> CheckIfTableColumnExistsAsync(string tableName, string columnName)
        {
            return await ExecuteScalarAsync<int>(string.Format(Dialect.CheckTableColumnExists, tableName, columnName)).ConfigureAwait(false) == 1;
        }

        #region ExecuteReaderAsync
        public override Task<TResult> ExecuteReaderAsync<TResult>(string commandText, Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReader(_useStatement, commandText, readerMapper);
        }

        public override Task<TResult> ExecuteReaderAsync<TResult>(string commandText, IDictionary<string, object> parameters, Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReader(_useStatement, commandText, parameters, readerMapper);
        }

        private Task<TResult> ExecuteReader<TResult>(string useStatement, string commandText, Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReader(useStatement, commandText, new Dictionary<string, object>(), readerMapper);
        }

        private async Task<TResult> ExecuteReader<TResult>(string useStatement, string commandText, IEnumerable<KeyValuePair<string, object>> parameters, Func<IDbReader, TResult> readerMapper)
        {
            using (var connection = await _connectionProvider.GetOpenConnectionAsync().ConfigureAwait(false))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = useStatement + commandText;
                parameters.ForEach(parameter => command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value ?? DBNull.Value)));

                using (var reader = command.ExecuteReader())
                {
                    var r = new DbReader(reader);
                    return readerMapper(r);
                }
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        public override Task ExecuteNonQueryAsync(string commandText)
        {
            return ExecuteNonQueryAsync(commandText, new Dictionary<string, object>());
        }

        public override Task ExecuteNonQueryAsync(string commandText, IDictionary<string, object> parameters)
        {
            return ExecuteNonQueryAsync(_useStatement, commandText, parameters);
        }

        private Task ExecuteNonQueryAsync(string useStatement, string commandText)
        {
            return ExecuteNonQueryAsync(useStatement, commandText, new Dictionary<string, object>());
        }

        private async Task ExecuteNonQueryAsync(string useStatement, string commandText, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            using (var connection = await _connectionProvider.GetOpenConnectionAsync().ConfigureAwait(false))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = useStatement + commandText;
                parameters.ForEach(
                    parameter =>
                        command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value ?? DBNull.Value)));

                    command.ExecuteNonQuery();
            }
        }

        #endregion

        #region ExecuteScalarAsync

        public override Task<TKey> ExecuteScalarAsync<TKey>(string commandText)
        {
            return ExecuteScalarAsync<TKey>(commandText, new Dictionary<string, object>());
        }

        public override Task<TKey> ExecuteScalarAsync<TKey>(string commandText, IDictionary<string, object> parameters)
        {
            return ExecuteScalarAsync<TKey>(_useStatement, commandText, parameters);
        }

        private Task<TKey> ExecuteScalarAsync<TKey>(string useStatement, string commandText)
        {
            return ExecuteScalarAsync<TKey>(useStatement, commandText, new Dictionary<string, object>());
        }

        private async Task<TKey> ExecuteScalarAsync<TKey>(string useStatement, string commandText, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            using (var connection = await _connectionProvider.GetOpenConnectionAsync().ConfigureAwait(false))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = useStatement + commandText;
                parameters.ForEach(
                    parameter =>
                            command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value ?? DBNull.Value)));

                var result = command.ExecuteScalar();
                if (typeof(TKey) == typeof(int))
                    return (TKey)(result ?? 0);

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

        #endregion
    }
}