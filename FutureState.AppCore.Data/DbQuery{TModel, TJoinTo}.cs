using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FutureState.AppCore.Data.Extensions;
using FutureState.AppCore.Data.Helpers;

namespace FutureState.AppCore.Data
{
    public class DbQuery<TModel, TJoinTo> : IDbQuery<TModel, TJoinTo> where TModel : class, new()
        where TJoinTo : class, new()
    {
        private readonly IDbMapper<TModel> _dbMapper;
        private readonly IDbProvider _dbProvider;
        private readonly string _joinTableName;
        private readonly JoinType _joinType;
        private readonly string _tableName;
        private string _joinExpression;
        private JoinExpressionVisitor _joinExpressionVisitor;
        private string _orderByClause;
        private OrderByExpressionVisitor _orderByExpressionVisitor;
        private Dictionary<string, object> _parameters;
        private string _skipTake;
        private string _whereClause;
        private WhereExpressionVisitor _whereExpressionVisitor;

        // Called if its a ManyToMany join, we can grab the join conditions from out attributes, no need for expression
        public DbQuery(IDbProvider dbProvider, JoinType joinType, IDbMapper<TModel> dbMapper)
        {
            _dbProvider = dbProvider;
            _joinType = joinType;
            _dbMapper = dbMapper;
            _tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            _joinTableName = typeof(TJoinTo).GetTypeInfo().Name.BuildTableName();
            _parameters = new Dictionary<string, object>();
            var joinFormat = "[{0}].[Id] == [{1}].[{2}Id]";
            var joinExpression = string.Format(joinFormat, _tableName, _joinTableName, _tableName.Singularize());
            _joinExpression = BuildJoinExpression(joinType, joinExpression);
        }

        public void Delete() => DeleteAsync().Wait();

        public IDbQuery<TModel, TJoinTo> On(Expression<Func<TModel, TJoinTo, object>> joinExpression)
        {
            if (_joinType == JoinType.ManyToMany)
                throw new NotSupportedException("The join type you selected is not compatible with the On statement.");

            _joinExpressionVisitor = new JoinExpressionVisitor().Visit(joinExpression);
            _joinExpression = BuildJoinExpression(_joinType, _joinExpressionVisitor.JoinExpression);
            return this;
        }

        public void Update(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters) => UpdateAsync(model, mapToDbParameters).Wait();

        public IDbQuery<TModel, TJoinTo> Where(Expression<Func<TModel, TJoinTo, object>> expression)
        {
            _whereExpressionVisitor = new WhereExpressionVisitor(_parameters).Visit(expression);
            _parameters = _whereExpressionVisitor.Parameters;

            if (string.IsNullOrEmpty(_whereClause))
                _whereClause = string.Format(_dbProvider.Dialect.Where, _whereExpressionVisitor.WhereExpression);
            else
                _whereClause += " AND " + _whereExpressionVisitor.WhereExpression;
            return this;
        }

        public IDbQuery<TModel, TJoinTo> OrderBy(Expression<Func<TModel, TJoinTo, object>> orderByExpression,
            OrderDirection direction)
        {
            _orderByExpressionVisitor = new OrderByExpressionVisitor().Visit(orderByExpression);

            _orderByClause = string.Format(
                _dbProvider.Dialect.OrderBy,
                _orderByExpressionVisitor.OrderByExpression,
                direction == OrderDirection.Ascending ? "ASC" : "DESC");

            return this;
        }

        public IEnumerable<TModel> Select() => SelectAsync().Result;

        public IEnumerable<TResult> Select<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc) => SelectAsync(mapperFunc).Result;

        public IDbQuery<TModel, TJoinTo> SkipTake(int skip, int take)
        {
            _skipTake = string.Format(_dbProvider.Dialect.SkipTake, skip, take);
            return this;
        }

        public Task<int> CountAsync() => _dbProvider.ExecuteScalarAsync<int>(ToStringCount(), _parameters);

        public int Count() => CountAsync().Result;

        public Task<IEnumerable<TModel>> SelectAsync() => SelectAsync(_dbMapper.BuildListFrom);

        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Func<IDbReader, IEnumerable<TResult>> mapperFunc)
            => _dbProvider.ExecuteReaderAsync(ToString(), _parameters, mapperFunc);

        public Task UpdateAsync(TModel model) => UpdateAsync(model, _dbMapper.BuildDbParametersFrom);

        public Task UpdateAsync(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters)
        {
            var dbFields = _dbMapper.FieldNames
                .Where(field => field != "ID")
                .Select(field => string.Format("[{0}] = @{0}", field))
                .ToList();

            var whereClause = GetExtendedWhereClause();
            var commandText = string.Format(_dbProvider.Dialect.UpdateJoin, _tableName, string.Join(",", dbFields),
                _joinExpression, whereClause);
            var parameters = _parameters.Union(mapToDbParameters(model))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            return _dbProvider.ExecuteNonQueryAsync(commandText, parameters);
        }

        public void Update(TModel model) => UpdateAsync(model).Wait();

        public Task DeleteAsync() => _dbProvider.ExecuteNonQueryAsync(ToStringDelete(), _parameters);

        public string ToStringCount()
        {
            return
                string.Format(_dbProvider.Dialect.SelectCountFromJoin, _tableName, _joinExpression,
                    GetExtendedWhereClause()).Trim();
        }

        public string ToStringDelete() => string.Format(_dbProvider.Dialect.DeleteFromJoin, _tableName, _joinExpression, _whereClause);

        public override string ToString() => string.Format(_dbProvider.Dialect.SelectFromJoin, _tableName, _joinExpression, GetExtendedWhereClause()).Trim();

        private string BuildJoinExpression(JoinType joinType, string joinString)
        {
            if (joinType == JoinType.Inner)
                return string.Format(_dbProvider.Dialect.InnerJoin, _joinTableName, joinString);
            if (joinType == JoinType.Left)
                return string.Format(_dbProvider.Dialect.LeftJoin, _joinTableName, joinString);

            if (joinType == JoinType.ManyToMany)
            {
                var names = new[] {_tableName, _joinTableName};
                Array.Sort(names, StringComparer.CurrentCulture);
                var manyManyTableName = string.Join("_", names);

                return string.Format(_dbProvider.Dialect.ManyToManyJoin,
                    _tableName, "Id", manyManyTableName, _tableName.Singularize() + "Id", _joinTableName,
                    _joinTableName.Singularize() + "Id");
            }
            throw new NotSupportedException("The join type you selected is not yet supported.");
        }

        private string GetExtendedWhereClause() => string.Join(" ", _whereClause, _orderByClause, _skipTake);
    }
}