using System;
using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public abstract class DbProviderBase : IDbProvider
    {
        public abstract string LoadSqlFile<TDbProvider>(string fileName);
        public abstract bool CheckIfDatabaseExists();
        public abstract void CreateDatabase();
        public abstract void DropDatabase();
        public abstract bool CheckIfTableExists(string tableName);
        public abstract TResult ExecuteReader<TResult>(string commandText, Func<IDbReader, TResult> readerMapper);

        public abstract TResult ExecuteReader<TResult>(string commandText, IDictionary<string, object> parameters,
                                                       Func<IDbReader, TResult> readerMapper);

        public abstract void ExecuteNonQuery(string commandText);
        public abstract void ExecuteNonQuery(string commandText, IDictionary<string, object> parameters);
        public abstract TKey ExecuteScalar<TKey>(string commandText);
        public abstract TKey ExecuteScalar<TKey>(string commandText, IDictionary<string, object> parameters);
        public abstract IDialect Dialect { get; }
        public string DatabaseName { get; set; }
    }
}