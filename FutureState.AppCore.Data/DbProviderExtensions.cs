using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FutureState.AppCore.Data.Attributes;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data
{
    public static class DbProviderExtensions
    {
        /// <summary>
        ///     Create a new record based on a Model
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="dbProvider">Database Provider</param>
        /// <param name="model">Model Object</param>
        /// <returns>The uniqueidentifier (Guid) of the newly created record.</returns>
        public static void Create<TModel>(this IDbProvider dbProvider, TModel model) where TModel : class, new()
        {
            Create(dbProvider, model, new AutoDbMapper<TModel>());
        }

        /// <summary>
        ///     Create a new record based on a Model
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="dbProvider">Database Provider</param>
        /// <param name="model">Model Object</param>
        /// <param name="dbMapper"></param>
        /// <returns>The uniqueidentifier (Guid) of the newly created record.</returns>
        public static void Create<TModel>(this IDbProvider dbProvider, TModel model, IDbMapper<TModel> dbMapper)
            where TModel : class, new()
        {
            var tableName = typeof (TModel).GetTypeInfo().Name.BuildTableName();
            var fieldNameList = dbMapper.FieldNames;
            var commandParams = dbMapper.BuildDbParametersFrom(model);

            var parameters = "@" + string.Join(",@", fieldNameList);
            var fields = string.Join(",", fieldNameList);
            var commandText = string.Format(dbProvider.Dialect.InsertInto, tableName, fields, parameters);

            dbProvider.ExecuteNonQuery(commandText, commandParams);

            UpdateManyToManyRelations(dbProvider, model, tableName, dbMapper);
        }

        /// <summary>
        ///     Query the Database for ALL records.
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="dbProvider">Database Provider</param>
        /// <returns>IEnumerable model</returns>
        public static IDbQuery<TModel> Query<TModel>(this IDbProvider dbProvider) where TModel : class, new()
        {
            return new DbQuery<TModel>(dbProvider, new AutoDbMapper<TModel>());
        }

        /// <summary>
        ///     Query the Database for ALL records.
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <typeparam name="TReturnType">The scalar return value</typeparam>
        /// <param name="dbProvider">Database Provider</param>
        /// <param name="propertyExpression">The expression to use for the query</param>
        /// <returns>IEnumerable model</returns>
        public static IDbScalar<TModel, TReturnType> Scalar<TModel, TReturnType>(this IDbProvider dbProvider,
            Expression<Func<TModel, TReturnType>> propertyExpression)
            where TModel : class, new()
        {
            return new DbScalar<TModel, TReturnType>(dbProvider, propertyExpression);
        }

        /// <summary>
        ///     Update the Database Record of a specified model.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="dbProvider">Database Provider</param>
        /// <param name="model">Model Object to update</param>
        /// <remarks>We're using the Id field to check the update.</remarks>
        public static void Update<TModel>(this IDbProvider dbProvider, TModel model) where TModel : class, new()
        {
            Update(dbProvider, model, new AutoDbMapper<TModel>());
        }

        /// <summary>
        ///     Update the Database Record of a specified model.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="dbProvider">Database Provider</param>
        /// <param name="model">Model Object to update</param>
        /// <param name="dbMapper">Used to map the data in the model object to parameters to be used in database calls</param>
        /// <remarks>We're using the Id field to check the update.</remarks>
        public static void Update<TModel>(this IDbProvider dbProvider, TModel model, IDbMapper<TModel> dbMapper)
            where TModel : class, new()
        {
            var tableName = typeof (TModel).GetTypeInfo().Name.BuildTableName();
            var fieldNameList = dbMapper.FieldNames;
            var commandParams = dbMapper.BuildDbParametersFrom(model);

            var setFieldText = fieldNameList.Select(field => string.Format("[{0}] = @{0}", field)).ToList();
            var whereClause = string.Format(dbProvider.Dialect.Where, "Id = @Id");
            var commandText = string.Format(dbProvider.Dialect.Update, tableName, string.Join(",", setFieldText), whereClause);

            dbProvider.ExecuteNonQuery(commandText, commandParams);
            UpdateManyToManyRelations(dbProvider, model, tableName, dbMapper);
        }

        /// <summary>
        ///     Delete the Database Record based on an expression
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="dbProvider">Database Provider</param>
        /// <param name="expression">The expression to use for the query</param>
        /// <remarks>THIS IS A HARD DELETE. When you run this method, the record is GONE!</remarks>
        public static void Delete<TModel>(this IDbProvider dbProvider, Expression<Func<TModel, object>> expression) where TModel : class, new()
        {
            var visitor = new WhereExpressionVisitor().Visit(expression);

            // this is a hard delete. soft deletes will happen in the repository layer.
            var tableName = typeof (TModel).GetTypeInfo().Name.BuildTableName();
            var whereClause = string.Format(dbProvider.Dialect.Where, visitor.WhereExpression);
            var commandText = string.Format(dbProvider.Dialect.DeleteFrom, tableName, whereClause);

            dbProvider.ExecuteNonQuery(commandText, visitor.Parameters);
        }

        /// <summary>
        ///     Updates all Join Tables based on the <see cref="ManyToManyAttribute" />
        /// </summary>
        /// <typeparam name="TModel">Object model Type</typeparam>
        /// <param name="dbProvider">DB Provider</param>
        /// <param name="model">Actual object model</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="dbMapper">Used to map the data in the model object to parameters to be used in database calls</param>
        private static void UpdateManyToManyRelations<TModel>(IDbProvider dbProvider, TModel model, string tableName, IDbMapper<TModel> dbMapper) where TModel : class, new()
        {
            var leftModel = dbMapper.BuildDbParametersFrom(model).FirstOrDefault(k => k.Key == "Id");
            var leftKey = typeof (TModel).Name.Replace("Model", string.Empty) + "Id";
            var parameters = new Dictionary<string, object> {{"@" + leftKey, leftModel.Value}};
            var manyToManyFields =
                typeof (TModel).GetRuntimeProperties().Where(property => property.GetCustomAttributes(typeof (ManyToManyAttribute), true).Any());

            foreach (var collection in manyToManyFields)
            {
//                if (!IsGenericList(collection))
//                {
//                    throw new ArgumentException("The property must be an ICollection<>");
//                }

                var joinTableName = GetJoinTableName(tableName, collection.Name);
                var deleteWhereClause = string.Format(dbProvider.Dialect.Where, string.Format("{0} = @{0}", leftKey));
                var deleteCommandText = string.Format(dbProvider.Dialect.DeleteFrom, joinTableName, deleteWhereClause);
                // Delete ALL records in the Join table associated with the `leftModel`
                dbProvider.ExecuteNonQuery(deleteCommandText, parameters);

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
                        var fieldsToInsert = string.Format(dbProvider.Dialect.JoinFields, leftKey, rightKey);
                        // "[{0}], [{1}]"
                        var parametersToSet = string.Format(dbProvider.Dialect.JoinParameters, leftKey, rightKey);
                        // "@{0}, @{1}"
                        var insertCommandText = string.Format(dbProvider.Dialect.InsertInto, joinTableName,
                            fieldsToInsert,
                            parametersToSet);
                        dbProvider.ExecuteNonQuery(insertCommandText, parameters);
                        // Remove the parameter for the next iteration.
                        parameters.Remove("@" + rightKey);
                    }
                }
            }
        }

        private static string GetJoinTableName(string tableName, string joinTableName)
        {
            var names = new[] {tableName, joinTableName};
            Array.Sort(names, StringComparer.CurrentCulture);
            return string.Join("_", names);
        }

        private static bool IsGenericList(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return type.GetTypeInfo().ImplementedInterfaces
                .Where(i => i.IsConstructedGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof (ICollection<>));
        }

//        static Type CheckForEnumerable(PropertyInfo propertyInfo)
//        {
//            var x = propertyInfo.GetType().GetTypeInfo().GenericTypeArguments[0];
//
//
//            foreach (Type intType in type.GenericTypeArguments)
//            {
//                if (intType.IsConstructedGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
//                {
//                    return intType.GenericTypeArguments()[0];
//                }
//            }
//            return null;
//        }
    }
}