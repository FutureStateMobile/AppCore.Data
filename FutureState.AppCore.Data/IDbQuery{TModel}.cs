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
        void Delete();
        Task DeleteAsync();
        TModel First();
        TResult First<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        Task<TModel> FirstAsync();
        Task<TResult> FirstAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        TModel FirstOrDefault();
        TResult FirstOrDefault<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        Task<TModel> FirstOrDefaultAsync();
        Task<TResult> FirstOrDefaultAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        IDbQuery<TModel, TJoinTo> Join<TJoinTo>() where TJoinTo : class, new();
        TModel Last();
        TResult Last<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        Task<TModel> LastAsync();
        Task<TResult> LastAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        TModel LastOrDefault();
        TResult LastOrDefault<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        Task<TModel> LastOrDefaultAsync();
        Task<TResult> LastOrDefaultAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        IDbQuery<TModel, TJoinTo> ManyToManyJoin<TJoinTo>() where TJoinTo : class, new();
        IDbQuery<TModel> OrderBy(Expression<Func<TModel, object>> orderByExpression, OrderDirection direction);
        IEnumerable<TModel> Select();
        IEnumerable<TResult> Select<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        Task<IEnumerable<TModel>> SelectAsync();
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        TModel Single();
        TResult Single<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        Task<TModel> SingleAsync();
        Task<TResult> SingleAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        TModel SingleOrDefault();
        TResult SingleOrDefault<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        Task<TModel> SingleOrDefaultAsync();
        Task<TResult> SingleOrDefaultAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        IDbQuery<TModel> SkipTake(int skip, int take);
        IList<TModel> ToList();
        IList<TResult> ToList<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        Task<IList<TModel>> ToListAsync();
        Task<IList<TResult>> ToListAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc);
        string ToStringCount();
        string ToStringDelete();
        string ToStringTruncate();
        void Truncate();
        void Update(TModel model);
        void Update(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters);
        Task UpdateAsync(TModel model);
        Task UpdateAsync(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters);
        IDbQuery<TModel> Where(Expression<Func<TModel, bool>> whereExpression);
    }
}