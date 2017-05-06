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
        private readonly IDbMapper<TModel> _dbMapper;
        private readonly IDbProvider _dbProvider;
        private readonly string _tableName;
        private string _orderByClause;
        private OrderByExpressionVisitor _orderByExpressionVisitor;
        private Dictionary<string, object> _parameters;
        private string _skipTake;
        private string _whereClause;
        private WhereExpressionVisitor _whereExpressionVisitor;

        public DbQuery(IDbProvider dbProvider, IDbMapper<TModel> dbMapper)
        {
            _dbProvider = dbProvider;
            _dbMapper = dbMapper;
            _tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            _parameters = new Dictionary<string, object>();
        }

        public IDbQuery<TModel> OrderBy(Expression<Func<TModel, object>> expression, OrderDirection direction)
        {
            _orderByExpressionVisitor = new OrderByExpressionVisitor().Visit(expression);

            _orderByClause = string.Format(
                _dbProvider.Dialect.OrderBy,
                _orderByExpressionVisitor.OrderByExpression,
                direction == OrderDirection.Ascending ? "ASC" : "DESC");

            return this;
        }

        public IDbQuery<TModel> SkipTake(int skip, int take)
        {
            _skipTake = string.Format(_dbProvider.Dialect.SkipTake, skip, take);
            return this;
        }

        public void Delete()
        {
            DeleteAsync().Wait();
        }

        public IDbQuery<TModel, TJoinTo> Join<TJoinTo>() where TJoinTo : class, new()
        {
            return new DbQuery<TModel, TJoinTo>(_dbProvider, JoinType.Left, _dbMapper);
        }

        public IDbQuery<TModel, TJoinTo> ManyToManyJoin<TJoinTo>() where TJoinTo : class, new()
        {
            return new DbQuery<TModel, TJoinTo>(_dbProvider, JoinType.ManyToMany, _dbMapper);
        }

        public IDbQuery<TModel> Where(Expression<Func<TModel, object>> expression)
        {
            _whereExpressionVisitor = new WhereExpressionVisitor(_parameters).Visit(expression);
            _parameters = _whereExpressionVisitor.Parameters;

            if (string.IsNullOrEmpty(_whereClause))
                _whereClause = string.Format(_dbProvider.Dialect.Where, _whereExpressionVisitor.WhereExpression);
            else
                _whereClause += " AND " + _whereExpressionVisitor.WhereExpression;
            return this;
        }

        public int Count()
        {
            return CountAsync().Result;
        }

        public Task<int> CountAsync()
        {
            return _dbProvider.ExecuteScalarAsync<int>(ToStringCount(), _parameters);
        }

        public Task<IEnumerable<TModel>> SelectAsync()
        {
            return SelectAsync(_dbMapper.BuildListFrom);
        }

        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc)
        {
            return _dbProvider.ExecuteReaderAsync(ToString(), _parameters, mapperFunc);
        }

        public IEnumerable<TModel> Select()
        {
            return SelectAsync().Result;
        }

        public IEnumerable<TResult> Select<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc)
        {
            return SelectAsync(mapperFunc).Result;
        }

        public Task UpdateAsync(TModel model)
        {
            return UpdateAsync(model, _dbMapper.BuildDbParametersFrom);
        }

        public Task UpdateAsync(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters)
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

        public void Update(TModel model)
        {
            UpdateAsync(model).Wait();
        }

        public void Update(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters)
        {
            UpdateAsync(model, mapToDbParameters).Wait();
        }

        public Task DeleteAsync()
        {
            return _dbProvider.ExecuteNonQueryAsync(ToStringDelete(), _parameters);
        }

        public void Truncate()
        {
            _dbProvider.ExecuteNonQueryAsync(ToStringTruncate());
        }

        public string ToStringCount()
        {
            return string.Format(_dbProvider.Dialect.SelectCountFrom, _tableName, GetExtendedWhereClause()).Trim();
        }

        public string ToStringDelete()
        {
            return string.Format(_dbProvider.Dialect.DeleteFrom, _tableName, _whereClause);
        }

        public string ToStringTruncate()
        {
            return string.Format(_dbProvider.Dialect.Truncate, _tableName);
        }

        public override string ToString()
        {
            return string.Format(_dbProvider.Dialect.SelectFrom, _tableName, GetExtendedWhereClause()).Trim();
        }

        private string GetExtendedWhereClause()
        {
            return string.Join(" ", _whereClause, _orderByClause, _skipTake);
        }
    }
}