using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite
{
    public interface IDbConnectionProvider
    {
        SqliteConnection GetOpenConnection();
    }
}