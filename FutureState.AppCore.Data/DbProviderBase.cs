using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FutureState.AppCore.Data.Attributes;
using FutureState.AppCore.Data.Config;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data
{
    public abstract class DbProviderBase : IDbProvider
    {
        private static readonly Dictionary<Type, string> _primaryKeys = new Dictionary<Type, string>();

        protected DbProviderBase()
        {
        }

        protected DbProviderBase(Action<DbConfiguration> config)
        {
            var dbConfig = new DbConfiguration(this);
            config(dbConfig);
        }

        public bool CheckIfDatabaseExists()
        {
            return CheckIfDatabaseExistsAsync().Result;
        }

        public abstract Task<bool> CheckIfDatabaseExistsAsync();

        public bool CheckIfTableColumnExists(string tableName, string columnName)
        {
            return CheckIfTableColumnExistsAsync(tableName, columnName).Result;
        }

        public abstract Task<bool> CheckIfTableColumnExistsAsync(string tableName, string columnName);

        public bool CheckIfTableExists(string tableName)
        {
            return CheckIfTableExistsAsync(tableName).Result;
        }

        public abstract Task<bool> CheckIfTableExistsAsync(string tableName);
        
        /// <summary>
        ///     Create a new record based on a Model
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="model">Model Object</param>
        /// <returns>The uniqueidentifier (Guid) of the newly created record.</returns>
        public void Create<TModel>(TModel model) where TModel : class, new()
        {
            CreateAsync(model).Wait();
        }

        /// <summary>
        ///     Create a new record based on a Model
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="model">Model Object</param>
        /// <param name="dbMapper"></param>
        /// <returns>The uniqueidentifier (Guid) of the newly created record.</returns>
        public void Create<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new()
        {
            CreateAsync(model, dbMapper).Wait();
        }

        /// <summary>
        ///     CreateAsync a new record based on a Model
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="model">Model Object</param>
        /// <returns>The uniqueidentifier (Guid) of the newly created record.</returns>
        public Task CreateAsync<TModel>(TModel model) where TModel : class, new()
        {
            return CreateAsync(model, new AutoDbMapper<TModel>());
        }

        /// <summary>
        ///     CreateAsync a new record based on a Model
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="model">Model Object</param>
        /// <param name="dbMapper"></param>
        /// <returns>The uniqueidentifier (Guid) of the newly created record.</returns>
        public async Task CreateAsync<TModel>(TModel model,
            IDbMapper<TModel> dbMapper)
            where TModel : class, new()
        {
            var tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            var fieldNameList = dbMapper.FieldNames;
            var commandParams = dbMapper.BuildDbParametersFrom(model);

            var parameters = "@" + string.Join(",@", fieldNameList);
            var fields = string.Join(",", fieldNameList);
            var commandText = string.Format(Dialect.InsertInto, tableName, fields, parameters);

            await ExecuteNonQueryAsync(commandText, commandParams).ConfigureAwait(false);

            await UpdateManyToManyRelationsAsync(model, tableName, dbMapper).ConfigureAwait(false);
        }

        public void CreateDatabase()
        {
            CreateDatabaseAsync().Wait();
        }

        public abstract Task CreateDatabaseAsync();
        public string DatabaseName { get; protected set; }

        /// <summary>
        ///     DeleteAsync the Database Record based on an expression
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="expression">The expression to use for the query</param>
        /// <remarks>THIS IS A HARD DELETE. When you run this method, the record is GONE!</remarks>
        public void Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class, new()
        {
            DeleteAsync(expression).Wait();
        }

        /// <summary>
        ///     DeleteAsync the Database Record based on an expression
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="expression">The expression to use for the query</param>
        /// <remarks>THIS IS A HARD DELETE. When you run this method, the record is GONE!</remarks>
        public Task DeleteAsync<TModel>(Expression<Func<TModel, bool>> expression)
            where TModel : class, new()
        {
            var visitor = new WhereExpressionVisitor().Visit(expression);

            // this is a hard delete. soft deletes will happen in the repository layer.
            var tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            var whereClause = string.Format(Dialect.Where, visitor.WhereExpression);
            var commandText = string.Format(Dialect.DeleteFrom, tableName, whereClause);

            return ExecuteNonQueryAsync(commandText, visitor.Parameters);
        }

        // Database specific stuff
        public abstract IDialect Dialect { get; }

        public void DropDatabase()
        {
            DropDatabaseAsync().Wait();
        }

        public abstract Task DropDatabaseAsync();

        public void ExecuteNonQuery(string commandText)
        {
            ExecuteNonQueryAsync(commandText).Wait();
        }

        public void ExecuteNonQuery(string commandText, IDictionary<string, object> parameters)
        {
            ExecuteNonQueryAsync(commandText, parameters).Wait();
        }

        // Used For Updates and Deletes
        public abstract Task ExecuteNonQueryAsync(string commandText);

        public abstract Task ExecuteNonQueryAsync(string commandText, IDictionary<string, object> parameters);

        public TResult ExecuteReader<TResult>(string commandText, Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReaderAsync(commandText, readerMapper).Result;
        }

        public TResult ExecuteReader<TResult>(string commandText, IDictionary<string, object> parameters,
            Func<IDbReader, TResult> readerMapper)
        {
            return ExecuteReaderAsync(commandText, parameters, readerMapper).Result;
        }

        // Used for Finds and Gets
        public abstract Task<TResult> ExecuteReaderAsync<TResult>(string commandText,
            Func<IDbReader, TResult> readerMapper);

        public abstract Task<TResult> ExecuteReaderAsync<TResult>(string commandText,
            IDictionary<string, object> parameters, Func<IDbReader, TResult> readerMapper);

        public TKey ExecuteScalar<TKey>(string commandText)
        {
            return ExecuteScalarAsync<TKey>(commandText).Result;
        }

        public TKey ExecuteScalar<TKey>(string commandText, IDictionary<string, object> parameters)
        {
            return ExecuteScalarAsync<TKey>(commandText, parameters).Result;
        }

        // Used for Creates
        public abstract Task<TKey> ExecuteScalarAsync<TKey>(string commandText);

        public abstract Task<TKey> ExecuteScalarAsync<TKey>(string commandText, IDictionary<string, object> parameters);

        public string LoadSqlFile<TDbProvider>(string fileName)
        {
            return LoadSqlFileAsync<TDbProvider>(fileName).Result;
        }

        public abstract Task<string> LoadSqlFileAsync<TDbProvider>(string fileName);

        /// <summary>
        ///     Query the Database for ALL records.
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <returns>IEnumerable model</returns>
        public IDbQuery<TModel> Query<TModel>() where TModel : class, new()
        {
            return new DbQuery<TModel>(this, new AutoDbMapper<TModel>());
        }

        /// <summary>
        ///     Query the Database for ALL records.
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <typeparam name="TReturnType">The scalar return value</typeparam>
        /// <param name="propertyExpression">The expression to use for the query</param>
        /// <returns>IEnumerable model</returns>
        public IDbScalar<TModel, TReturnType> Scalar<TModel, TReturnType>(Expression<Func<TModel, TReturnType>> propertyExpression)
            where TModel : class, new()
        {
            return new DbScalar<TModel, TReturnType>(this, propertyExpression);
        }

        /// <summary>
        ///     UpdateAsync the Database Record of a specified model.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model Object to update</param>
        public void Update<TModel>(TModel model) where TModel : class, new()
        {
            UpdateAsync(model, new AutoDbMapper<TModel>()).Wait();
        }

        /// <summary>
        ///     UpdateAsync the Database Record of a specified model.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model Object to update</param>
        /// <param name="dbMapper">Used to map the data in the model object to parameters to be used in database calls</param>
        public void Update<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new()
        {
            UpdateAsync(model, dbMapper).Wait();
        }

        /// <summary>
        ///     UpdateAsync the Database Record of a specified model.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model Object to update</param>
        public Task UpdateAsync<TModel>(TModel model) where TModel : class, new()
        {
            return UpdateAsync(model, new AutoDbMapper<TModel>());
        }

        /// <summary>
        ///     UpdateAsync the Database Record of a specified model.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model Object to update</param>
        /// <param name="dbMapper">Used to map the data in the model object to parameters to be used in database calls</param>
        public async Task UpdateAsync<TModel>(TModel model,
            IDbMapper<TModel> dbMapper)
            where TModel : class, new()
        {
            var modelType = typeof(TModel);
            var identifierName = GetPrimaryKeyName(modelType);

            var tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            var fieldNameList = dbMapper.FieldNames;
            var commandParams = dbMapper.BuildDbParametersFrom(model);

            var setFieldText = fieldNameList.Select(field => string.Format("[{0}] = @{0}", field)).ToList();
            var whereClause = string.Format(Dialect.Where, string.Format("{0} = @{0}", identifierName));
            var commandText = string.Format(Dialect.Update, tableName, string.Join(",", setFieldText),
                whereClause);

            await ExecuteNonQueryAsync(commandText, commandParams).ConfigureAwait(false);
            await UpdateManyToManyRelationsAsync(model, tableName, dbMapper).ConfigureAwait(false);
        }



        //private static bool IsGenericList(Type type)
        //{
        //        .Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>));
        //        .Where(i => i.IsConstructedGenericType)
        //    return type.GetTypeInfo().ImplementedInterfaces
        //        throw new ArgumentNullException("type");
        //    if (type == null)
        //}

        #region DbConfiguration Region

        internal void AddOrUpdatePrimaryKey(Type type, string key)
        {
            if (_primaryKeys.ContainsKey(type))
                _primaryKeys[type] = key;
            else
                _primaryKeys.Add(type, key);
        }

        #endregion

        private string GetPrimaryKeyName(Type modelType)
        {
            string identifierName;
            if (_primaryKeys.ContainsKey(modelType)) identifierName = _primaryKeys[modelType];
            else
            {
                identifierName = modelType.GetRuntimeProperties()
                                     .FirstOrDefault(property => property.GetCustomAttributes(true).Any(a => a.GetType().Name == nameof(PrimaryKeyAttribute)))
                                     ?.Name ?? "Id";
                _primaryKeys[modelType] = identifierName;
            }

            return identifierName;
        }

        private static string GetJoinTableName(string tableName, string joinTableName)
        {
            var names = new[] {tableName, joinTableName};
            Array.Sort(names, StringComparer.CurrentCulture);
            return string.Join("_", names);
        }

        /// <summary>
        ///     Updates all Join Tables based on the <see cref="ManyToManyAttribute" />
        /// </summary>
        /// <typeparam name="TModel">Object model Type</typeparam>
        /// <param name="model">Actual object model</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="dbMapper">Used to map the data in the model object to parameters to be used in database calls</param>
        private async Task UpdateManyToManyRelationsAsync<TModel>(TModel model,
            string tableName, IDbMapper<TModel> dbMapper) where TModel : class, new()
        {
            var leftModel = dbMapper.BuildDbParametersFrom(model).FirstOrDefault(k => k.Key == "Id");
            var leftKey = typeof(TModel).Name.Replace("Model", string.Empty) + "Id";
            var parameters = new Dictionary<string, object> {{"@" + leftKey, leftModel.Value}};
            var manyToManyFields =
                typeof(TModel).GetRuntimeProperties()
                    .Where(property => property.GetCustomAttributes(true).Any(a => a.GetType().Name == nameof(ManyToManyAttribute)));

            foreach (var collection in manyToManyFields)
            {
                //                if (!IsGenericList(collection))
                //                {
                //                    throw new ArgumentException("The property must be an ICollection<>");
                //                }

                var joinTableName = GetJoinTableName(tableName, collection.Name);
                var deleteWhereClause = string.Format(Dialect.Where, string.Format("{0} = @{0}", leftKey));
                var deleteCommandText = string.Format(Dialect.DeleteFrom, joinTableName, deleteWhereClause);
                // DeleteAsync ALL records in the Join table associated with the `leftModel`
                await ExecuteNonQueryAsync(deleteCommandText, parameters).ConfigureAwait(false);

                var manyToManyCollection = collection.PropertyType.GenericTypeArguments.FirstOrDefault();
                var listValues = (IEnumerable<object>) collection.GetValue(model, null);
                if (listValues == null) continue;

                foreach (var value in listValues.Distinct())
                {
                    if (manyToManyCollection == null)
                        throw new ArgumentException();
                    var rightProperties = manyToManyCollection.GetRuntimeProperties();
                    var manyToManyCollectionName = manyToManyCollection.Name.Replace("Model", string.Empty);
                    foreach (var rightProperty in rightProperties)
                    {
                        var rightPropertyName = rightProperty.Name;
                        if (rightPropertyName != "Id")
                            continue; // short circuit the loop if we're not dealing with the primary key.
                        var rightKey = manyToManyCollectionName + rightPropertyName;
                        var rightValue = rightProperty.GetValue(value, null);
                        parameters.Add("@" + rightKey, rightValue);
                        var fieldsToInsert = string.Format(Dialect.JoinFields, leftKey, rightKey);
                        // "[{0}], [{1}]"
                        var parametersToSet = string.Format(Dialect.JoinParameters, leftKey, rightKey);
                        // "@{0}, @{1}"
                        var insertCommandText = string.Format(Dialect.InsertInto, joinTableName,
                            fieldsToInsert,
                            parametersToSet);
                        await ExecuteNonQueryAsync(insertCommandText, parameters).ConfigureAwait(false);
                        // Remove the parameter for the next iteration.
                        parameters.Remove("@" + rightKey);
                    }
                }
            }
        }
    }
}