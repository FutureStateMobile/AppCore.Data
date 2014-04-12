using System;
using System.Data;
using System.Data.Common;

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

        public IDbConnection GetOpenConnection()
        {
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(_connectionProviderName);

            DbConnection connection = dbFactory.CreateConnection();
            if (connection == null) throw new Exception("Could not create a database connection.");

            connection.ConnectionString = String.Format(_connectionString);
            connection.Open();

            return connection;
        }
    }
}