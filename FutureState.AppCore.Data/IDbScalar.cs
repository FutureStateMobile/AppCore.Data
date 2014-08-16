using System;
using System.Linq.Expressions;

namespace FutureState.AppCore.Data
{
    public interface IDbScalar<TModel, out TType> where TModel : class, new()
    {
        IDbScalar<TModel, TType> Where ( Expression<Func<TModel, object>> whereExpression );
        TType Max();
        TType Min();
        TType Sum();
        string ToStringMax();
        string ToStringMin();
        string ToStringSum();
    }
}