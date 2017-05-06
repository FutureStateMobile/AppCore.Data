using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FutureState.AppCore.Data
{
    public interface IDbScalar<TModel, TType> where TModel : class, new()
    {
        IDbScalar<TModel, TType> Where ( Expression<Func<TModel, object>> whereExpression );
        Task<TType> MaxAsync();
        Task<TType> MinAsync();
        Task<TType> SumAsync();
        TType Max();
        TType Min();
        TType Sum();

        string ToStringMax();
        string ToStringMin();
        string ToStringSum();
    }
}