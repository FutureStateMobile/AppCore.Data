using System.Data;
using System.Threading.Tasks;

namespace FutureState.AppCore.Data
{
    public interface IDbConnectionProvider
    {
        Task<IDbConnection> GetOpenConnectionAsync();
    }
}