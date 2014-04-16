using System;
using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IDbProvider
    {
        // Database specific stuff
        IDialect Dialect { get; }
        string DatabaseName { get; set; }
        string LoadSqlFile(string fileName);
        bool CheckIfDatabaseExists();
        void CreateDatabase();
        void DropDatabase();
        bool CheckIfTableExists(string tableName);

        // Used for Finds and Gets
        TResult ExecuteReader<TResult>(string commandText, Func<IDbReader, TResult> readerMapper);
        TResult ExecuteReader<TResult>(string commandText, IDictionary<string, object> parameters, Func<IDbReader, TResult> readerMapper);

        // Used For Updates and Deletes
        void ExecuteNonQuery(string commandText);
        void ExecuteNonQuery(string commandText, IDictionary<string, object> parameters);

        // Used for Creates
        TKey ExecuteScalar<TKey>(string commandText);
        TKey ExecuteScalar<TKey>(string commandText, IDictionary<string, object> parameters);
    }
}