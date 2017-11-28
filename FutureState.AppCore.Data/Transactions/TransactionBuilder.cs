using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FutureState.AppCore.Data.Attributes;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data.Transactions
{
    public abstract class TransactionBuilder : IDbChange
    {
        protected readonly IDialect Dialect;

        protected TransactionBuilder(IDialect dialect)
        {
            Dialect = dialect;
            Commands = new List<TransactionCommand>();
        }

        internal ICollection<TransactionCommand> Commands { get; set; }

        /// <summary>
        ///     Create a new record based on a Model
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="model">Model Object</param>
        /// <returns>The uniqueidentifier (Guid) of the newly created record.</returns>
        public void Create<TModel>(TModel model) where TModel : class, new() => Create(model, new AutoDbMapper<TModel>());

        /// <summary>
        ///     Create a new record based on a Model
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="model">Model Object</param>
        /// <param name="dbMapper"></param>
        /// <returns>The uniqueidentifier (Guid) of the newly created record.</returns>
        public void Create<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new()
        {
            var tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            var fieldNameList = dbMapper.FieldNames;
            var commandParams = dbMapper.BuildDbParametersFrom(model);

            var parameters = "@" + string.Join(",@", fieldNameList);
            var fields = string.Join(",", fieldNameList);
            var commandText = string.Format(Dialect.InsertInto, tableName, fields, parameters);
            Commands.Add(new TransactionCommand(commandText, commandParams));
            UpdateManyToManyRelationsAsync(model, tableName,dbMapper);
        }

        /// <summary>
        ///     DeleteAsync the Database Record based on an expression
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="expression">The expression to use for the query</param>
        /// <remarks>THIS IS A HARD DELETE. When you run this method, the record is GONE!</remarks>
        public void Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class, new()
        {
            var visitor = new WhereExpressionVisitor().Visit(expression);
            var tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            var whereClause = string.Format(Dialect.Where, visitor.WhereExpression);
            var commandText = string.Format(Dialect.DeleteFrom, tableName, whereClause);
            Commands.Add(new TransactionCommand(commandText,visitor.Parameters));
        }

        public void ExecuteNonQuery(string commandText) => ExecuteNonQuery(commandText, new Dictionary<string, object>());

        public void ExecuteNonQuery(string commandText, IDictionary<string, object> parameters) => Commands.Add(new TransactionCommand(commandText, parameters));


        /// <summary>
        ///     UpdateAsync the Database Record of a specified model.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model Object to update</param>
        public void Update<TModel>(TModel model) where TModel : class, new() => Update(model, new AutoDbMapper<TModel>());

        /// <summary>
        ///     UpdateAsync the Database Record of a specified model.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model Object to update</param>
        /// <param name="dbMapper">Used to map the data in the model object to parameters to be used in database calls</param>
        public void Update<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new()
        {
            var modelType = typeof(TModel);
            var identifierName = modelType.GetPrimaryKeyName();

            var tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            var fieldNameList = dbMapper.FieldNames;
            var commandParams = dbMapper.BuildDbParametersFrom(model);

            var setFieldText = fieldNameList.Select(field => string.Format("[{0}] = @{0}", field)).ToList();
            var whereClause = string.Format(Dialect.Where, string.Format("{0} = @{0}", identifierName));
            var commandText = string.Format(Dialect.Update, tableName, string.Join(",", setFieldText),
                whereClause);
            Commands.Add(new TransactionCommand(commandText, commandParams));
            UpdateManyToManyRelationsAsync(model, tableName, dbMapper);
        }

        /// <summary>
        ///     Update the record if it doesn't exist, otherwise create a new one.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model Object to create or update</param>
        public void CreateOrUpdate<TModel>(TModel model) where TModel : class, new() => CreateOrUpdate(model, new AutoDbMapper<TModel>());


        /// <summary>
        ///     Update the record if it doesn't exist, otherwise create a new one.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="model">Model Object to create or update</param>
        /// <param name="dbMapper">Used to map the data in the model object to parameters to be used in database calls</param>
        public abstract void CreateOrUpdate<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new();

        /// <summary>
        ///     Updates all Join Tables based on the <see cref="ManyToManyAttribute" />
        /// </summary>
        /// <typeparam name="TModel">Object model Type</typeparam>
        /// <param name="model">Actual object model</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="dbMapper">Used to map the data in the model object to parameters to be used in database calls</param>
        protected void UpdateManyToManyRelationsAsync<TModel>(TModel model,
            string tableName, IDbMapper<TModel> dbMapper) where TModel : class, new()
        {
            var primaryKey = model.GetType().GetPrimaryKeyName();
            var leftModel = dbMapper.BuildDbParametersFrom(model).FirstOrDefault(k => k.Key == primaryKey);
            var leftKey = typeof(TModel).Name.Replace("Model", string.Empty) + primaryKey;
            var parameters = new Dictionary<string, object> { { "@" + leftKey, leftModel.Value } };
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
                Commands.Add(new TransactionCommand(deleteCommandText, parameters));

                var manyToManyCollection = collection.PropertyType.GenericTypeArguments.FirstOrDefault();
                var listValues = (IEnumerable<object>)collection.GetValue(model, null);
                if (listValues == null) continue;

                foreach (var value in listValues.Distinct())
                {
                    if (manyToManyCollection == null)
                        throw new ArgumentException();
                    var rightProperties = manyToManyCollection.GetRuntimeProperties();
                    var manyToManyCollectionName = manyToManyCollection.Name.Replace("Model", string.Empty);
                    foreach (var rightProperty in rightProperties)
                    {
                        var insertParams = new Dictionary<string, object> {{ "@" + leftKey, parameters["@" + leftKey] }};
                        var rightPropertyName = rightProperty.Name;
                        if (rightPropertyName != primaryKey)
                            continue; // short circuit the loop if we're not dealing with the primary key.
                        var rightKey = manyToManyCollectionName + rightPropertyName;
                        var rightValue = rightProperty.GetValue(value, null);
                        insertParams.Add("@" + rightKey, rightValue);
                        var fieldsToInsert = string.Format(Dialect.JoinFields, leftKey, rightKey);
                        // "[{0}], [{1}]"
                        var parametersToSet = string.Format(Dialect.JoinParameters, leftKey, rightKey);
                        // "@{0}, @{1}"
                        var insertCommandText = string.Format(Dialect.InsertInto, joinTableName,
                            fieldsToInsert,
                            parametersToSet);
                        Commands.Add(new TransactionCommand(insertCommandText, insertParams));
                        // Remove the parameter for the next iteration.
                        //parameters.Remove("@" + rightKey);
                    }
                }
            }
        }
        private static string GetJoinTableName(string tableName, string joinTableName)
        {
            var names = new[] { tableName, joinTableName };
            Array.Sort(names, StringComparer.CurrentCulture);
            return string.Join("_", names);
        }
    }
}