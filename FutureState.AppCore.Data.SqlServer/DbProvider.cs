using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FutureState.AppCore.Data.Config;
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

        public DbProvider(IDbConnectionProvider connectionProvider, string databaseName, Action<DbConfiguration> dbConfig):base(dbConfig)
        {
            DatabaseName = databaseName;
            _connectionProvider = connectionProvider;
            _useStatement = string.Format(Dialect.UseDatabase, databaseName);
        }

        public override async Task CreateOrUpdateAsync<TModel>(TModel model, IDbMapper<TModel> dbMapper)
        {
            var modelType = typeof(TModel);
            var tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            var fieldNameList = dbMapper.FieldNames;
            var commandParams = dbMapper.BuildDbParametersFrom(model);

            var insertParams = "@" + string.Join(",@", fieldNameList);
            var insertFields = string.Join(",", fieldNameList);
            var updateFields = string.Join(",", fieldNameList.Select(field => string.Format("[{0}] = @{0}", field)).ToList());
            var whereClause = string.Format(Dialect.Where, string.Format("{0} = @{0}", modelType.GetPrimaryKeyName()));

            var commandText = string.Format(Dialect.CreateOrUpdate, 
                tableName,
                updateFields,
                whereClause,
                insertFields,
                insertParams);

            await ExecuteNonQueryAsync(commandText, commandParams).ConfigureAwait(false);
            await UpdateManyToManyRelationsAsync(model, tableName, dbMapper).ConfigureAwait(false);
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

        public override async Task RunInTransactionAsync(Action<IDbChange> dbChange)
        {
            var transaction = new SqlServerTransactionBuilder(Dialect) ;
            dbChange(transaction);
            using (var connection = await _connectionProvider.GetOpenConnectionAsync().ConfigureAwait(false))
            using(var trans = connection.BeginTransaction())
            using (var command = connection.CreateCommand())
            {
                command.Transaction = trans;
                try
                {
                    command.CommandType = CommandType.Text;
                    foreach (var cmd in transaction.Commands)
                    {
                        command.CommandText = _useStatement + cmd.CommandText;
                        cmd.CommandParameters.ForEach(parameter =>
                        {
                            command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value ?? DBNull.Value));
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

        public override async Task<bool> CheckIfDatabaseExistsAsync() => await ExecuteScalarAsync<int>("", string.Format(Dialect.CheckDatabaseExists, DatabaseName)).ConfigureAwait(false) == 1;

        public override Task CreateDatabaseAsync() => ExecuteNonQueryAsync("", string.Format(Dialect.CreateDatabase, DatabaseName));

        public override Task DropDatabaseAsync() => ExecuteNonQueryAsync("", string.Format(Dialect.DropDatabase, DatabaseName));

        public override async Task<bool> CheckIfTableExistsAsync(string tableName) => await ExecuteScalarAsync<int>(string.Format(Dialect.CheckTableExists, tableName)).ConfigureAwait(false) == 1;

        public override async Task<bool> CheckIfTableColumnExistsAsync(string tableName, string columnName) => await ExecuteScalarAsync<int>(string.Format(Dialect.CheckTableColumnExists, tableName, columnName)).ConfigureAwait(false) == 1;

        #region ExecuteReaderAsync

        public override Task<TResult> ExecuteReaderAsync<TResult>(string commandText, IDictionary<string, object> parameters, Func<IDbReader, TResult> readerMapper) => ExecuteReader(_useStatement, commandText, parameters, readerMapper);

        private async Task<TResult> ExecuteReader<TResult>(string useStatement, string commandText, IEnumerable<KeyValuePair<string, object>> parameters, Func<IDbReader, TResult> readerMapper)
        {
            using (var connection = await _connectionProvider.GetOpenConnectionAsync().ConfigureAwait(false))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = useStatement + commandText;
                parameters.ForEach(parameter => command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value ?? DBNull.Value)));
                TResult result;
                using (var reader = command.ExecuteReader())
                {
                    var r = new DbReader(reader);
                    result =  readerMapper(r);
                }
                return result;
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        public override Task ExecuteNonQueryAsync(string commandText, IDictionary<string, object> parameters) => ExecuteNonQueryAsync(_useStatement, commandText, parameters);

        private Task ExecuteNonQueryAsync(string useStatement, string commandText) => ExecuteNonQueryAsync(useStatement, commandText, new Dictionary<string, object>());

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

        public override Task<TKey> ExecuteScalarAsync<TKey>(string commandText, IDictionary<string, object> parameters) => ExecuteScalarAsync<TKey>(_useStatement, commandText, parameters);

        private Task<TKey> ExecuteScalarAsync<TKey>(string useStatement, string commandText) => ExecuteScalarAsync<TKey>(useStatement, commandText, new Dictionary<string, object>());

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