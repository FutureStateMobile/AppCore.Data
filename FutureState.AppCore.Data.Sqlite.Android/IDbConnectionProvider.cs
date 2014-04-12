using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite.Android
{
    public interface IDbConnectionProvider
    {
        SqliteConnection GetOpenConnection();
    }
}