using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FutureState.AppCore.Data
{
    public interface IDbProvider
    {
        // Database specific stuff
        IDialect Dialect { get; }
        string DatabaseName { get; set; }
        bool CheckIfDatabaseExists();
        Task<bool> CheckIfDatabaseExistsAsync();
        bool CheckIfTableColumnExists(string tableName, string columnName);
        Task<bool> CheckIfTableColumnExistsAsync(string tableName, string columnName);
        bool CheckIfTableExists(string tableName);
        Task<bool> CheckIfTableExistsAsync(string tableName);
        void CreateDatabase();
        Task CreateDatabaseAsync();
        void DropDatabase();
        Task DropDatabaseAsync();
        void ExecuteNonQuery(string commandText);
        void ExecuteNonQuery(string commandText, IDictionary<string, object> parameters);

        // Used For Updates and Deletes
        Task ExecuteNonQueryAsync(string commandText);
        Task ExecuteNonQueryAsync(string commandText, IDictionary<string, object> parameters);

        TResult ExecuteReader<TResult>(string commandText, Func<IDbReader, TResult> readerMapper);

        TResult ExecuteReader<TResult>(string commandText, IDictionary<string, object> parameters,
            Func<IDbReader, TResult> readerMapper);

        // Used for Finds and Gets
        Task<TResult> ExecuteReaderAsync<TResult>(string commandText, Func<IDbReader, TResult> readerMapper);

        Task<TResult> ExecuteReaderAsync<TResult>(string commandText, IDictionary<string, object> parameters,
            Func<IDbReader, TResult> readerMapper);

        TKey ExecuteScalar<TKey>(string commandText);
        TKey ExecuteScalar<TKey>(string commandText, IDictionary<string, object> parameters);

        // Used for Creates
        Task<TKey> ExecuteScalarAsync<TKey>(string commandText);
        Task<TKey> ExecuteScalarAsync<TKey>(string commandText, IDictionary<string, object> parameters);
        Task<string> LoadSqlFileAsync<TDbProvider>(string fileName);
        string LoadSqlFile<TDbProvider>(string fileName);
    }
}