using System.IO;
using System.Reflection;

namespace FutureState.AppCore.Data.Sqlite
{
    public abstract class SqliteDbProviderBase : DbProviderBase
    {
        protected const string RootSqlScriptPath = "FutureState.AppCore.Data.Sqlite.SqlScripts.";
        private IDialect _dialect;

        public override sealed IDialect Dialect
        {
            get { return _dialect ?? (_dialect = new SqliteDialect()); }
        }

        public override string LoadSqlFile<TDbProvider>(string fileName)
        {
            var sqlStatement = string.Empty;
            
            using ( var resourceStream = typeof( TDbProvider ).GetTypeInfo().Assembly.GetManifestResourceStream( RootSqlScriptPath + fileName ) )
            {
                if (resourceStream != null)
                {
                    sqlStatement = new StreamReader(resourceStream).ReadToEnd();
                }
            }

            return sqlStatement;
        }
    }
}