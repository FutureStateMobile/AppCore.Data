using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite.Windows
{
    public interface IDbConnectionProvider
    {
        SqliteConnection GetOpenConnection();
    }
}