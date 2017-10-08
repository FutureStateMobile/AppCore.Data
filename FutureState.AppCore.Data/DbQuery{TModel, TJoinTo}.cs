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
    public class DbQuery<TModel, TJoinTo> : DbQuery<TModel>, IDbQuery<TModel, TJoinTo> where TModel : class, new()
        where TJoinTo : class, new()
    {
        private readonly string _joinTableName;
        private readonly JoinType _joinType;
        private string _joinExpression;
        private JoinExpressionVisitor _joinExpressionVisitor;
        private OrderByExpressionVisitor _orderByExpressionVisitor;
        private WhereExpressionVisitor _whereExpressionVisitor;

        // Called if its a ManyToMany join, we can grab the join conditions from out attributes, no need for expression
        public DbQuery(IDbProvider dbProvider, JoinType joinType, IDbMapper<TModel> dbMapper) : base(dbProvider, dbMapper)
        {
            const string joinFormat = "[{0}].[Id] == [{1}].[{2}Id]";

            _joinType = joinType;
            _joinTableName = typeof(TJoinTo).GetTypeInfo().Name.BuildTableName();
            var joinExpression = string.Format(joinFormat, _tableName, _joinTableName, _tableName.Singularize());
            _joinExpression = BuildJoinExpression(joinType, joinExpression);
        }

        public IDbQuery<TModel, TJoinTo> On(Expression<Func<TModel, TJoinTo, object>> joinExpression)
        {
            if (_joinType == JoinType.ManyToMany)
                throw new NotSupportedException("The join type you selected is not compatible with the On statement.");

            _joinExpressionVisitor = new JoinExpressionVisitor().Visit(joinExpression);
            _joinExpression = BuildJoinExpression(_joinType, _joinExpressionVisitor.JoinExpression);
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

        public new IDbQuery<TModel, TJoinTo> SkipTake(int skip, int take)
        {
            base.SkipTake(skip, take);
            return this;
        }

        public IDbQuery<TModel, TJoinTo> Where(Expression<Func<TModel, TJoinTo, bool>> expression)
        {
            _whereExpressionVisitor = new WhereExpressionVisitor(_parameters).Visit(expression);
            _parameters = _whereExpressionVisitor.Parameters;

            if (string.IsNullOrEmpty(_whereClause))
                _whereClause = string.Format(_dbProvider.Dialect.Where, _whereExpressionVisitor.WhereExpression);
            else
                _whereClause += " AND " + _whereExpressionVisitor.WhereExpression;
            return this;
        }

        public override string ToStringCount() => string.Format(_dbProvider.Dialect.SelectCountFromJoin, _tableName, _joinExpression, GetExtendedWhereClause()).Trim();

        public override string ToStringDelete() => string.Format(_dbProvider.Dialect.DeleteFromJoin, _tableName, _joinExpression, _whereClause);

        public override Task UpdateAsync(TModel model, Func<TModel, IDictionary<string, object>> mapToDbParameters)
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
    }
}