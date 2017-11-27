using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FutureState.AppCore.Data.Config;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data.Sqlite
{
    public abstract class SqliteDbProviderBase : DbProviderBase
    {
        protected const string RootSqlScriptPath = "FutureState.AppCore.Data.Sqlite.SqlScripts.";
        private IDialect _dialect;

        protected SqliteDbProviderBase() { }

        protected SqliteDbProviderBase(Action<DbConfiguration> config) : base(config) { }

        public sealed override IDialect Dialect => _dialect ?? (_dialect = new SqliteDialect());

        public override async Task CreateOrUpdateAsync<TModel>(TModel model, IDbMapper<TModel> dbMapper)
        {
            var tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            var fieldNameList = dbMapper.FieldNames;
            var commandParams = dbMapper.BuildDbParametersFrom(model);

            var parameters = "@" + string.Join(",@", fieldNameList);
            var fields = string.Join(",", fieldNameList);
            var commandText = string.Format(Dialect.CreateOrUpdate, tableName, fields, parameters);

            await ExecuteNonQueryAsync(commandText, commandParams).ConfigureAwait(false);
            await UpdateManyToManyRelationsAsync(model, tableName, dbMapper).ConfigureAwait(false);
        }

        public override async Task<string> LoadSqlFileAsync<TDbProvider>(string fileName)
        {
            var sqlStatement = string.Empty;
            
            using ( var resourceStream = typeof( TDbProvider ).GetTypeInfo().Assembly.GetManifestResourceStream( RootSqlScriptPath + fileName ) )
            {
                if (resourceStream != null)
                {
                    sqlStatement = await new StreamReader(resourceStream).ReadToEndAsync().ConfigureAwait(false);
                }
            }

            return sqlStatement;
        }
    }
}