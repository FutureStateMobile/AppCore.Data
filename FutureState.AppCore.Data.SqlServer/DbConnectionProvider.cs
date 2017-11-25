using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace FutureState.AppCore.Data.SqlServer
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly string _connectionProviderName;
        private readonly string _connectionString;

        public DbConnectionProvider(string connectionString, string connectionProviderName)
        {
            _connectionString = connectionString;
            _connectionProviderName = connectionProviderName;
        }

        public async Task<IDbConnection> GetOpenConnectionAsync()
        {
            var dbFactory = DbProviderFactories.GetFactory(_connectionProviderName);

            var connection = dbFactory.CreateConnection();
            if (connection == null) throw new Exception("Could not create a database connection.");

            connection.ConnectionString = string.Format(_connectionString);
            await  connection.OpenAsync().ConfigureAwait(false);

            return connection;
        }
    }
}