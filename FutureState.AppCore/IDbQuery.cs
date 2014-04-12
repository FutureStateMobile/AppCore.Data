using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FutureState.AppCore.Data
{
    public interface IDbQuery<TModel>
        where TModel : class, new()
    {
        IDbQuery<TModel, TJoinTo> Join<TJoinTo>(JoinType joinType) where TJoinTo : class, new();

        IDbQuery<TModel, TJoinTo> Join<TJoinTo>(JoinType joinType, Expression<Func<TModel, TJoinTo, object>> joinExpression)
            where TJoinTo : class, new();

        IDbQuery<TModel> Where(Expression<Func<TModel, object>> whereExpression);
        IDbQuery<TModel> OrderBy(Expression<Func<TModel, object>> orderByExpression, OrderDirection direction);
        IDbQuery<TModel> SkipTake(int skip, int take);
        IEnumerable<TModel> Select();
        void Delete();
    }

    public interface IDbQuery<TModel, TJoinTo>
        where TModel : class
        where TJoinTo : class, new()
    {
        IDbQuery<TModel, TJoinTo> Where(Expression<Func<TModel, TJoinTo, object>> whereExpression);
        IDbQuery<TModel, TJoinTo> OrderBy(Expression<Func<TModel, TJoinTo, object>> orderByExpression, OrderDirection direction);
        IDbQuery<TModel, TJoinTo> SkipTake(int skip, int take);
        IEnumerable<TModel> Select();
        void Delete();
    }
}