using System.IO;
using System.Reflection;

namespace FutureState.AppCore.Data.Sqlite
{
    public abstract class SqliteDbProviderBase : DbProviderBase
    {
        protected const string RootSqlScriptPath = "FutureState.AppCore.Data.Sqlite.SqlScripts.";
        protected const string CheckTableExists = "SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'";

        public override sealed IDialect Dialect
        {
            get { return new SqliteDialect(); }
        }

        public override string LoadSqlFile(string fileName)
        {
            var sqlStatement = string.Empty;

            using (
                var resourceStream =
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(RootSqlScriptPath + fileName))
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