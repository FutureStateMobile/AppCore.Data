using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FutureState.AppCore.Data
{
    public abstract class DbProviderBase : IDbProvider
    {
        // Database specific stuff
        public abstract IDialect Dialect { get; }
        public string DatabaseName { get; set; }
        public abstract Task<string> LoadSqlFileAsync<TDbProvider>(string fileName);

        public string LoadSqlFile<TDbProvider>(string fileName)
        {
            return LoadSqlFileAsync<TDbProvider>(fileName).Result;
        }
        public abstract Task<bool> CheckIfDatabaseExistsAsync();

        public bool CheckIfDatabaseExists()
        {
            return CheckIfDatabaseExistsAsync().Result;
        }

        public abstract Task CreateDatabaseAsync();
        public abstract Task DropDatabaseAsync();

        public void CreateDatabase()
        {
            CreateDatabaseAsync().Wait();
        }

        public void DropDatabase()
        {
            DropDatabaseAsync().Wait();
        }

        public bool CheckIfTableExists(string tableName)
        {
            return CheckIfTableExistsAsync(tableName).Result;
        }

        public bool CheckIfTableColumnExists(string tableName, string columnName)
        {
            return CheckIfTableColumnExistsAsync(tableName, columnName).Result;
        }

        public abstract Task<bool> CheckIfTableExistsAsync(string tableName);
        public abstract Task<bool> CheckIfTableColumnExistsAsync(string tableName, string columnName);

        // Used for Finds and Gets
        public abstract Task<TResult> ExecuteReaderAsync<TResult>(string commandText,
            Func<IDbReader, TResult> readerMapper);

        public abstract Task<TResult> ExecuteReaderAsync<TResult>(string commandText,
            IDictionary<string, object> parameters, Func<IDbReader, TResult> readerMapper);

        public TResult ExecuteReader<TResult>(string commandText, Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReaderAsync(commandText, readerMapper).Result;
        }

        public TResult ExecuteReader<TResult>(string commandText, IDictionary<string, object> parameters,
            Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReaderAsync(commandText, parameters, readerMapper).Result;
        }

        // Used For Updates and Deletes
        public abstract Task ExecuteNonQueryAsync(string commandText);
        public abstract Task ExecuteNonQueryAsync(string commandText, IDictionary<string, object> parameters);

        public void ExecuteNonQuery(string commandText)
        {
            ExecuteNonQueryAsync(commandText).Wait();
        }

        public void ExecuteNonQuery(string commandText, IDictionary<string, object> parameters)
        {
            ExecuteNonQueryAsync(commandText, parameters).Wait();
        }

        // Used for Creates
        public abstract Task<TKey> ExecuteScalarAsync<TKey>(string commandText);
        public abstract Task<TKey> ExecuteScalarAsync<TKey>(string commandText, IDictionary<string, object> parameters);

        public TKey ExecuteScalar<TKey>(string commandText)
        {
            return ExecuteScalarAsync<TKey>(commandText).Result;
        }

        public TKey ExecuteScalar<TKey>(string commandText, IDictionary<string, object> parameters)
        {
            return ExecuteScalarAsync<TKey>(commandText, parameters).Result;
        }
    }
}