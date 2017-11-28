using FutureState.AppCore.Data.Extensions;
using FutureState.AppCore.Data.Transactions;

namespace FutureState.AppCore.Data.Sqlite
{
    public class SQLiteTransactionBuilder : TransactionBuilder
    {
        public SQLiteTransactionBuilder(IDialect dialect) : base(dialect)
        {
        }

        public override void CreateOrUpdate<TModel>(TModel model, IDbMapper<TModel> dbMapper)
        {
            var tableName = typeof(TModel).Name.BuildTableName();
            var fieldNameList = dbMapper.FieldNames;
            var commandParams = dbMapper.BuildDbParametersFrom(model);

            var parameters = "@" + string.Join(",@", fieldNameList);
            var fields = string.Join(",", fieldNameList);
            var commandText = string.Format(Dialect.CreateOrUpdate, tableName, fields, parameters);
            Commands.Add(new TransactionCommand(commandText, commandParams));
            UpdateManyToManyRelationsAsync(model, tableName, dbMapper);
        }
    }
}