using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FutureState.AppCore.Data
{
    public interface IDbQuery<TModel, TJoinTo> where TModel : class where TJoinTo : class, new()
    {
        int Count();
        Task<int> CountAsync();
        void Delete();
        Task DeleteAsync();
        IDbQuery<TModel, TJoinTo> On(Expression<Func<TModel, TJoinTo, object>> joinExpression);

        IDbQuery<TModel, TJoinTo> OrderBy(Expression<Func<TModel, TJoinTo, object>> orderByExpression,
            OrderDirection direction);

        IEnumerable<TModel> Select();
        IEnumerable<TResult> Select<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);

        Task<IEnumerable<TModel>> SelectAsync();
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        IDbQuery<TModel, TJoinTo> SkipTake(int skip, int take);
        string ToStringCount();
        string ToStringDelete();
        void Update(TModel model);
        void Update(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters);
        Task UpdateAsync(TModel model);
        Task UpdateAsync(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters);
        IDbQuery<TModel, TJoinTo> Where(Expression<Func<TModel, TJoinTo, object>> whereExpression);
    }
}