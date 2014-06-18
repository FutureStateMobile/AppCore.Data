using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace FutureState.AppCore.Data.SqlServer
{
    public class DbProvider : DbProviderBase
    {
        private const string RootSqlScriptPath = "FutureState.AppCore.Data.SqlServer.SqlScripts.";
        private static string _useStatement;
        private readonly IDbConnectionProvider _connectionProvider;
        private IDialect _dialect;

        public DbProvider(IDbConnectionProvider connectionProvider, string databaseName)
        {
            DatabaseName = databaseName;
            _connectionProvider = connectionProvider;
            _useStatement = string.Format(Dialect.UseDatabase, databaseName);
        }

        public override sealed IDialect Dialect
        {
            get { return _dialect ?? ( _dialect = new SqlServerDialect() ); }
        }

        public override string LoadSqlFile<TDbProvider>(string fileName)
        {
            var sqlStatement = string.Empty;

            using (
                var resourceStream =
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(RootSqlScriptPath + fileName))
            {
                if (resourceStream != null)
                {
                    sqlStatement = new StreamReader(resourceStream).ReadToEnd();
                }
            }

            return sqlStatement;
        }

        public override bool CheckIfDatabaseExists()
        {
            return ExecuteScalar<int>("USE master; ", string.Format(Dialect.CheckDatabaseExists, DatabaseName)) == 1;
        }

        public override void CreateDatabase()
        {
            ExecuteNonQuery("USE master; ", string.Format(Dialect.CreateDatabase, DatabaseName));
        }

        public override void DropDatabase()
        {
            ExecuteNonQuery("USE master; ", string.Format(Dialect.DropDatabase, DatabaseName));
        }

        public override bool CheckIfTableExists(string tableName)
        {
            return ExecuteScalar<int>(string.Format(Dialect.CheckTableExists, tableName)) == 1;
        }

        public override bool CheckIfTableColumnExists ( string tableName, string columnName )
        {
            return ExecuteScalar<int>( string.Format( Dialect.CheckTableColumnExists, tableName, columnName ) ) == 1;
        }

        #region ExecuteReader

        public override TResult ExecuteReader<TResult>(string commandText, Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReader(_useStatement, commandText, readerMapper);
        }

        public override TResult ExecuteReader<TResult>(string commandText,
                                                       IDictionary<string, object> parameters,
                                                       Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReader(_useStatement, commandText, parameters, readerMapper);
        }

        private TResult ExecuteReader<TResult>(string useStatement, string commandText,
                                               Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReader(useStatement, commandText, new Dictionary<string, object>(), readerMapper);
        }

        private TResult ExecuteReader<TResult>(string useStatement,
                                               string commandText,
                                               IEnumerable<KeyValuePair<string, object>> parameters,
                                               Func<IDbReader, TResult> readerMapper)
        {
            using (var connection = _connectionProvider.GetOpenConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = useStatement + commandText;
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value ?? DBNull.Value));
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
            ExecuteNonQuery(_useStatement, commandText, parameters);
        }

        private void ExecuteNonQuery(string useStatement, string commandText)
        {
            ExecuteNonQuery(useStatement, commandText, new Dictionary<string, object>());
        }

        private void ExecuteNonQuery(string useStatement, string commandText,
                                     IEnumerable<KeyValuePair<string, object>> parameters)
        {
            using (var connection = _connectionProvider.GetOpenConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = useStatement + commandText;
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value ?? DBNull.Value));
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
            return ExecuteScalar<TKey>(_useStatement, commandText, parameters);
        }

        private TKey ExecuteScalar<TKey>(string useStatement, string commandText)
        {
            return ExecuteScalar<TKey>(useStatement, commandText, new Dictionary<string, object>());
        }

        private TKey ExecuteScalar<TKey>(string useStatement, string commandText,
                                         IEnumerable<KeyValuePair<string, object>> parameters)
        {
            using (var connection = _connectionProvider.GetOpenConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = useStatement + commandText;
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value ?? DBNull.Value));
                    }

                    return (TKey) command.ExecuteScalar();
                }
            }
        }

        #endregion
    }
}