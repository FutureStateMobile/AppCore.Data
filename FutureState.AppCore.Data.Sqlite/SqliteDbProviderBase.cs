using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace FutureState.AppCore.Data.Sqlite
{
    public abstract class SqliteDbProviderBase : DbProviderBase
    {
        protected const string RootSqlScriptPath = "FutureState.AppCore.Data.Sqlite.SqlScripts.";
        private IDialect _dialect;

        public sealed override IDialect Dialect => _dialect ?? (_dialect = new SqliteDialect());

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