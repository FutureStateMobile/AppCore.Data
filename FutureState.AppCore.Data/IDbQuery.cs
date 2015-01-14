using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FutureState.AppCore.Data
{
    public interface IDbQuery<TModel> where TModel : class, new()
    {
        IDbQuery<TModel, TJoinTo> Join<TJoinTo>() where TJoinTo : class, new();
        IDbQuery<TModel, TJoinTo> ManyToManyJoin<TJoinTo>() where TJoinTo : class, new();
        IDbQuery<TModel> Where(Expression<Func<TModel, object>> whereExpression);
        IDbQuery<TModel> OrderBy(Expression<Func<TModel, object>> orderByExpression, OrderDirection direction);
        IDbQuery<TModel> SkipTake(int skip, int take);
        int Count();
        IEnumerable<TModel> Select();
        IEnumerable<TResult> Select<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        void Update(TModel model);
        void Update(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters);
        void Delete();
        void Truncate();
        string ToStringCount();
        string ToStringDelete();
        string ToStringTruncate();
    }

    public interface IDbQuery<TModel, TJoinTo> where TModel : class where TJoinTo : class, new()
    {
        IDbQuery<TModel, TJoinTo> On(Expression<Func<TModel, TJoinTo, object>> joinExpression);
        IDbQuery<TModel, TJoinTo> Where(Expression<Func<TModel, TJoinTo, object>> whereExpression);
        IDbQuery<TModel, TJoinTo> OrderBy(Expression<Func<TModel, TJoinTo, object>> orderByExpression, OrderDirection direction);
        IDbQuery<TModel, TJoinTo> SkipTake(int skip, int take);
        int Count ();
        IEnumerable<TModel> Select();
        IEnumerable<TResult> Select<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        void Update(TModel model);
        void Update(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters);
        void Delete ();
        string ToStringCount ();
        string ToStringDelete ();
    }
}