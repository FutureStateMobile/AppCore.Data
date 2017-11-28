﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FutureState.AppCore.Data
{
    public interface IDbProvider : IDbChange
    {
        string DatabaseName { get; }
        IDialect Dialect { get; }
        bool CheckIfDatabaseExists();
        Task<bool> CheckIfDatabaseExistsAsync();
        bool CheckIfTableColumnExists(string tableName, string columnName);
        Task<bool> CheckIfTableColumnExistsAsync(string tableName, string columnName);
        bool CheckIfTableExists(string tableName);
        Task<bool> CheckIfTableExistsAsync(string tableName);
        Task CreateAsync<TModel>(TModel model) where TModel : class, new();
        Task CreateAsync<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new();
        void CreateDatabase();
        Task CreateDatabaseAsync();
        Task CreateOrUpdateAsync<TModel>(TModel model) where TModel : class, new();
        Task CreateOrUpdateAsync<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new();
        Task DeleteAsync<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class, new();
        void DropDatabase();
        Task DropDatabaseAsync();
        Task ExecuteNonQueryAsync(string commandText);
        Task ExecuteNonQueryAsync(string commandText, IDictionary<string, object> parameters);
        TResult ExecuteReader<TResult>(string commandText, Func<IDbReader, TResult> readerMapper);
        TResult ExecuteReader<TResult>(string commandText, IDictionary<string, object> parameters, Func<IDbReader, TResult> readerMapper);
        Task<TResult> ExecuteReaderAsync<TResult>(string commandText, Func<IDbReader, TResult> readerMapper);
        Task<TResult> ExecuteReaderAsync<TResult>(string commandText, IDictionary<string, object> parameters, Func<IDbReader, TResult> readerMapper);
        TKey ExecuteScalar<TKey>(string commandText);
        TKey ExecuteScalar<TKey>(string commandText, IDictionary<string, object> parameters);
        Task<TKey> ExecuteScalarAsync<TKey>(string commandText);
        Task<TKey> ExecuteScalarAsync<TKey>(string commandText, IDictionary<string, object> parameters);
        string LoadSqlFile<TDbProvider>(string fileName);
        Task<string> LoadSqlFileAsync<TDbProvider>(string fileName);
        IDbQuery<TModel> Query<TModel>() where TModel : class, new();
        void RunInTransaction(Action<IDbChange> transaction);
        Task RunInTransactionAsync(Action<IDbChange> dbChange);
        IDbScalar<TModel, TReturnType> Scalar<TModel, TReturnType>(Expression<Func<TModel, TReturnType>> propertyExpression) where TModel : class, new();
        void Update<TModel>(TModel model) where TModel : class, new();
        Task UpdateAsync<TModel>(TModel model) where TModel : class, new();
        Task UpdateAsync<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new();
    }
}