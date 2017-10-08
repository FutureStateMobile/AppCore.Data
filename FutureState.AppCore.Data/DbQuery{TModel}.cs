using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data
{
    public class DbQuery<TModel> : IDbQuery<TModel> where TModel : class, new()
    {
        protected readonly IDbMapper<TModel> _dbMapper;
        protected readonly IDbProvider _dbProvider;
        protected string _orderByClause;
        protected Dictionary<string, object> _parameters;
        protected string _skipTake;
        protected string _tableName;
        protected string _whereClause;
        private OrderByExpressionVisitor _orderByExpressionVisitor;
        private WhereExpressionVisitor _whereExpressionVisitor;

        public DbQuery(IDbProvider dbProvider, IDbMapper<TModel> dbMapper)
        {
            _dbMapper = dbMapper;
            _tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            _dbProvider = dbProvider;
            _parameters = new Dictionary<string, object>();
        }

        public int Count() => CountAsync().Result;

        public Task<int> CountAsync() => _dbProvider.ExecuteScalarAsync<int>(ToStringCount(), _parameters);

        public void Delete() => DeleteAsync().Wait();

        public Task DeleteAsync() => _dbProvider.ExecuteNonQueryAsync(ToStringDelete(), _parameters);
        
        public TResult First<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => SelectAsync(mapperFunc).Result.First();

        public TModel First() => SelectAsync().Result.First();

        public async Task<TResult> FirstAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => (await SelectAsync(mapperFunc).ConfigureAwait(false)).First();

        public async Task<TModel> FirstAsync() => (await SelectAsync().ConfigureAwait(false)).First();
        
        public TResult FirstOrDefault<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => FirstAsync(mapperFunc).Result;

        public TModel FirstOrDefault() => FirstOrDefaultAsync().Result;

        public async Task<TResult> FirstOrDefaultAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => (await SelectAsync(mapperFunc).ConfigureAwait(false)).FirstOrDefault();
        
        public async Task<TModel> FirstOrDefaultAsync() => (await SelectAsync().ConfigureAwait(false)).FirstOrDefault();
        
        public IDbQuery<TModel, TJoinTo> Join<TJoinTo>() where TJoinTo : class, new() => new DbQuery<TModel, TJoinTo>(_dbProvider, JoinType.Left, _dbMapper);

        public TModel Last() => SelectAsync().Result.Last();

        public TResult Last<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => SelectAsync(mapperFunc).Result.Last();

        public async Task<TModel> LastAsync() => (await SelectAsync().ConfigureAwait(false)).Last();

        public async Task<TResult> LastAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => (await SelectAsync(mapperFunc).ConfigureAwait(false)).Last();

        public TModel LastOrDefault() => SelectAsync().Result.LastOrDefault();

        public TResult LastOrDefault<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => SelectAsync(mapperFunc).Result.LastOrDefault();

        public async Task<TModel> LastOrDefaultAsync() => (await SelectAsync().ConfigureAwait(false)).LastOrDefault();

        public async Task<TResult> LastOrDefaultAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => (await SelectAsync(mapperFunc).ConfigureAwait(false)).LastOrDefault();

        public IDbQuery<TModel, TJoinTo> ManyToManyJoin<TJoinTo>() where TJoinTo : class, new() => new DbQuery<TModel, TJoinTo>(_dbProvider, JoinType.ManyToMany, _dbMapper);

        public IDbQuery<TModel> OrderBy(Expression<Func<TModel, object>> expression, OrderDirection direction)
        {
            _orderByExpressionVisitor = new OrderByExpressionVisitor().Visit(expression);

            _orderByClause = string.Format(
                _dbProvider.Dialect.OrderBy,
                _orderByExpressionVisitor.OrderByExpression,
                direction == OrderDirection.Ascending ? "ASC" : "DESC");

            return this;
        }

        public IEnumerable<TResult> Select<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => SelectAsync(mapperFunc).Result;
        
        public IEnumerable<TModel> Select() => SelectAsync().Result;

        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => _dbProvider.ExecuteReaderAsync(ToString(), _parameters, mapperFunc);
        public TModel Single() => SelectAsync().Result.Single();

        public TResult Single<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => SelectAsync(mapperFunc).Result.Single();

        public async Task<TModel> SingleAsync() => (await SelectAsync().ConfigureAwait(false)).Single();

        public async Task<TResult> SingleAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => (await SelectAsync(mapperFunc).ConfigureAwait(false)).Single();

        public TModel SingleOrDefault() => SelectAsync().Result.SingleOrDefault();

        public TResult SingleOrDefault<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => SelectAsync(mapperFunc).Result.SingleOrDefault();

        public async Task<TModel> SingleOrDefaultAsync() => (await SelectAsync().ConfigureAwait(false)).SingleOrDefault();

        public async Task<TResult> SingleOrDefaultAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => (await SelectAsync(mapperFunc).ConfigureAwait(false)).SingleOrDefault();

        public Task<IEnumerable<TModel>> SelectAsync() => SelectAsync(_dbMapper.BuildListFrom);

        public IDbQuery<TModel> SkipTake(int skip, int take)
        {
            _skipTake = string.Format(_dbProvider.Dialect.SkipTake, skip, take);
            return this;
        }

        public IList<TModel> ToList() => SelectAsync().Result.ToList();

        public IList<TResult> ToList<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => SelectAsync(mapperFunc).Result.ToList();

        public async Task<IList<TModel>> ToListAsync() => (await SelectAsync().ConfigureAwait(false)).ToList();

        public async Task<IList<TResult>> ToListAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => (await SelectAsync(mapperFunc).ConfigureAwait(false)).ToList();

        public virtual string ToStringCount() => string.Format(_dbProvider.Dialect.SelectCountFrom, _tableName, GetExtendedWhereClause()).Trim();

        public virtual string ToStringDelete() => string.Format(_dbProvider.Dialect.DeleteFrom, _tableName, _whereClause);
        
        public string ToStringTruncate() => string.Format(_dbProvider.Dialect.Truncate, _tableName);

        public void Truncate() => _dbProvider.ExecuteNonQueryAsync(ToStringTruncate());

        public void Update(TModel model) => UpdateAsync(model).Wait();

        public void Update(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters) => UpdateAsync(model, mapToDbParameters).Wait();
        
        public Task UpdateAsync(TModel model) => UpdateAsync(model, _dbMapper.BuildDbParametersFrom);

        public virtual Task UpdateAsync(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters)
        {
            var dbFields = _dbMapper.FieldNames
                .Where(field => field != "ID")
                .Select(field => string.Format("[{0}] = @{0}", field)).ToList();

            var whereClause = GetExtendedWhereClause();
            var commandText = string.Format(_dbProvider.Dialect.Update, _tableName, string.Join(",", dbFields),
                whereClause);
            var parameters = _parameters.Union(mapToDbParameters(model))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            return _dbProvider.ExecuteNonQueryAsync(commandText, parameters);
        }

        public IDbQuery<TModel> Where(Expression<Func<TModel, bool>> expression)
        {
            _whereExpressionVisitor = new WhereExpressionVisitor(_parameters).Visit(expression);
            _parameters = _whereExpressionVisitor.Parameters;

            if (string.IsNullOrEmpty(_whereClause))
                _whereClause = string.Format(_dbProvider.Dialect.Where, _whereExpressionVisitor.WhereExpression);
            else
                _whereClause += " AND " + _whereExpressionVisitor.WhereExpression;
            return this;
        }

        public override string ToString() => string.Format(_dbProvider.Dialect.SelectFrom, _tableName, GetExtendedWhereClause()).Trim();

        protected string GetExtendedWhereClause() => string.Join(" ", _whereClause, _orderByClause, _skipTake);
    }
}