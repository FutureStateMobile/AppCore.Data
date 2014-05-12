using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FutureState.AppCore.Data
{
    public class DbQuery<TModel> : IDbQuery<TModel>
        where TModel : class, new()
    {
        private readonly IDbProvider _dbProvider;
        private readonly AutoMapper<TModel> _mapper;
        private readonly string _tableName;
        private string _orderByClause;
        private OrderByExpressionVisitor _orderByExpressionVisitor;
        private Dictionary<string, object> _parameters;
        private string _skipTake;
        private string _whereClause;
        private WhereExpressionVisitor _whereExpressionVisitor;

        public DbQuery(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
            _mapper = new AutoMapper<TModel>();
            _tableName = GetTableName(typeof (TModel));
            _parameters = new Dictionary<string, object>();
        }

        public IDbQuery<TModel> OrderBy(Expression<Func<TModel, object>> expression, OrderDirection direction)
        {
            _orderByExpressionVisitor = new OrderByExpressionVisitor().Visit(expression);
            _orderByClause = string.Format(_dbProvider.Dialect.OrderBy,
                                           _orderByExpressionVisitor.OrderByExpression,
                                           (direction == OrderDirection.Ascending) ? "ASC" : "DESC");
            return this;
        }

        public IDbQuery<TModel> SkipTake(int skip, int take)
        {
            _skipTake = string.Format(_dbProvider.Dialect.SkipTake, skip, take);
            return this;
        }

        // Used for OneToMany and OneToOne joins, we will use and Expression to build the Join clause
        public IDbQuery<TModel, TJoinTo> Join<TJoinTo>(JoinType joinType,
                                                       Expression<Func<TModel, TJoinTo, object>> joinExpression)
            where TJoinTo : class, new()
        {
            return new DbQuery<TModel, TJoinTo>(_dbProvider, joinType, joinExpression);
        }

        // Used For ManyToMany Joins, we will use our ManyToMany attribute to build this join clause
        public IDbQuery<TModel, TJoinTo> Join<TJoinTo>(JoinType joinType) where TJoinTo : class, new()
        {
            return new DbQuery<TModel, TJoinTo>(_dbProvider, joinType);
        }

        public IDbQuery<TModel> Where(Expression<Func<TModel, object>> expression)
        {
            _whereExpressionVisitor = new WhereExpressionVisitor().Visit(expression);
            _whereClause = string.Format(_dbProvider.Dialect.Where, _whereExpressionVisitor.WhereExpression);
            _parameters = _whereExpressionVisitor.Parameters;

            return this;
        }

        public IEnumerable<TModel> Select()
        {
            var commandText = string.Format(_dbProvider.Dialect.SelectFrom, _tableName, GetExtendedWhereClause()).Trim();
            return _dbProvider.ExecuteReader(commandText, _parameters, _mapper.BuildQueueFrom);
        }

        public void Delete()
        {
            var commandText = string.Format(_dbProvider.Dialect.DeleteFrom, _tableName, _whereClause);
            _dbProvider.ExecuteNonQuery(commandText, _parameters);
        }

        public override string ToString()
        {
            return string.Format(_dbProvider.Dialect.SelectFrom, _tableName, GetExtendedWhereClause()).Trim();
        }

        private string GetExtendedWhereClause()
        {
            return String.Join(" ", _whereClause, _orderByClause, _skipTake);
        }

        private static string GetTableName(MemberInfo type)
        {
            return type.Name.Replace("Model", "s");
        }
    }

    public class DbQuery<TModel, TJoinTo> : IDbQuery<TModel, TJoinTo>
        where TModel : class, new()
        where TJoinTo : class, new()
    {
        private readonly IDbProvider _dbProvider;
        private readonly string _joinTableName;
        private readonly JoinType _joinType;
        private readonly AutoMapper<TModel> _mapper;
        private readonly string _tableName;
        private JoinExpressionVisitor _joinExpressionVisitor;
        private Dictionary<string, object> _parameters;

        // Called if its a ManyToMany join, we can grab the join conditions from out attributes, no need for expression
        public DbQuery(IDbProvider dbProvider, JoinType joinType)
        {
            _dbProvider = dbProvider;
            _joinType = joinType;
            _mapper = new AutoMapper<TModel>();
            _tableName = GetTableName(typeof (TModel));
            _joinTableName = GetTableName(typeof (TJoinTo));
            _parameters = new Dictionary<string, object>();
        }

        // Called for OneToOne and OneToMany joins, we need to look at the express to build Join clause
        public DbQuery(IDbProvider dbProvider, JoinType joinType,
                       Expression<Func<TModel, TJoinTo, object>> joinExpression)
            : this(dbProvider, joinType)
        {
            _joinExpressionVisitor = new JoinExpressionVisitor().Visit(joinType, joinExpression);
        }

        public IDbQuery<TModel, TJoinTo> Where(Expression<Func<TModel, TJoinTo, object>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public IDbQuery<TModel, TJoinTo> OrderBy(Expression<Func<TModel, TJoinTo, object>> orderByExpression,
                                                 OrderDirection direction)
        {
            throw new NotImplementedException();
        }

        public IDbQuery<TModel, TJoinTo> SkipTake(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TModel> Select()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        private static string GetTableName(MemberInfo type)
        {
            return type.Name.Replace("Model", "s");
        }
    }
}