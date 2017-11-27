using System;
using System.Linq.Expressions;

namespace FutureState.AppCore.Data
{
    public interface IDbQuery<TModel, TJoinTo> : IDbQuery<TModel> where TModel : class, new() where TJoinTo : class, new()
    {
        IDbQuery<TModel, TJoinTo> On(Expression<Func<TModel, TJoinTo, object>> joinExpression);
        IDbQuery<TModel, TJoinTo> OrderBy(Expression<Func<TModel, TJoinTo, object>> orderByExpression, OrderDirection direction);
        new IDbQuery<TModel, TJoinTo> SkipTake(int skip, int take);
        IDbQuery<TModel, TJoinTo> Where(Expression<Func<TModel, TJoinTo, bool>> whereExpression);
    }
}