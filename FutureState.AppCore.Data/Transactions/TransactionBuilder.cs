using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
    }
}