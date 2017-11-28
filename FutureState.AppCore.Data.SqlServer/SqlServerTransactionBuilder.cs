using System.Linq;
using System.Reflection;
using FutureState.AppCore.Data.Extensions;
using FutureState.AppCore.Data.Transactions;

namespace FutureState.AppCore.Data.SqlServer
{
    public class SqlServerTransactionBuilder : TransactionBuilder
    {
        public SqlServerTransactionBuilder(IDialect dialect):base(dialect)
        {
        }

        public override void CreateOrUpdate<TModel>(TModel model, IDbMapper<TModel> dbMapper)
        {
            var modelType = typeof(TModel);
            var tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            var fieldNameList = dbMapper.FieldNames;
            var commandParams = dbMapper.BuildDbParametersFrom(model);

            var insertParams = "@" + string.Join(",@", fieldNameList);
            var insertFields = string.Join(",", fieldNameList);
            var updateFields = string.Join(",", fieldNameList.Select(field => string.Format((string) "[{0}] = @{0}", (object) field)).ToList());
            var whereClause = string.Format(Dialect.Where, string.Format("{0} = @{0}", modelType.GetPrimaryKeyName()));

            var commandText = string.Format(Dialect.CreateOrUpdate,
                tableName,
                updateFields,
                whereClause,
                insertFields,
                insertParams);

            Commands.Add(new TransactionCommand(commandText, commandParams));
        }
    }
}