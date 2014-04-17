using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite.Touch
{
    public interface IDbConnectionProvider
    {
        SqliteConnection GetOpenConnection();
    }
}