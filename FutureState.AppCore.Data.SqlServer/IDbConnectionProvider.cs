using System.Data;

namespace FutureState.AppCore.Data.SqlServer
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetOpenConnection();
    }
}