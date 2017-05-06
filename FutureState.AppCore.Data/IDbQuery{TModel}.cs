using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FutureState.AppCore.Data
{
    public interface IDbQuery<TModel> where TModel : class, new()
    {
        int Count();
        Task<int> CountAsync();
        Task DeleteAsync();
        void Delete();
        IDbQuery<TModel, TJoinTo> Join<TJoinTo>() where TJoinTo : class, new();
        IDbQuery<TModel, TJoinTo> ManyToManyJoin<TJoinTo>() where TJoinTo : class, new();
        IDbQuery<TModel> OrderBy(Expression<Func<TModel, object>> orderByExpression, OrderDirection direction);
        IEnumerable<TModel> Select();
        IEnumerable<TResult> Select<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        Task<IEnumerable<TModel>> SelectAsync();
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        IDbQuery<TModel> SkipTake(int skip, int take);
        string ToStringCount();
        string ToStringDelete();
        string ToStringTruncate();
        void Truncate();
        void Update(TModel model);
        void Update(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters);
        Task UpdateAsync(TModel model);
        Task UpdateAsync(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters);
        IDbQuery<TModel> Where(Expression<Func<TModel, object>> whereExpression);
    }
}