using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FutureState.AppCore.Data.Extensions;
using FutureState.AppCore.Data.Helpers;

namespace FutureState.AppCore.Data
{
    public class DbQuery<TModel> : IDbQuery<TModel> where TModel : class, new()
    {
        private readonly IDbProvider _dbProvider;
        private readonly IDbMapper<TModel> _dbMapper;
        private readonly string _tableName;
        private string _orderByClause;
        private string _whereClause;
        private Dictionary<string, object> _parameters;
        private string _skipTake;
        private OrderByExpressionVisitor _orderByExpressionVisitor;
        private WhereExpressionVisitor _whereExpressionVisitor;

//        public DbQuery(IDbProvider dbProvider)
//        {
//            _dbProvider = dbProvider;
//            _mapper = new AutoMapper<TModel>();
//            _tableName = GetTableName(typeof (TModel).GetTypeInfo());
//            _parameters = new Dictionary<string, object>();
//        }

        public DbQuery ( IDbProvider dbProvider, IDbMapper<TModel> dbMapper  )
        {
            _dbProvider = dbProvider;
            _dbMapper = dbMapper;
            _tableName = typeof( TModel ).GetTypeInfo().Name.BuildTableName();
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

        public IDbQuery<TModel, TJoinTo> InnerJoin<TJoinTo>() where TJoinTo : class, new()
        {
            return new DbQuery<TModel, TJoinTo>( _dbProvider, JoinType.Inner);
        }

        public IDbQuery<TModel, TJoinTo> LeftJoin<TJoinTo> () where TJoinTo : class, new()
        {
            return new DbQuery<TModel, TJoinTo>( _dbProvider, JoinType.Left );
        }

        public IDbQuery<TModel, TJoinTo> ManyJoin<TJoinTo> () where TJoinTo : class, new()
        {
            return new DbQuery<TModel, TJoinTo>( _dbProvider, JoinType.ManyToMany );
        }

        // Used for OneToMany and OneToOne joins, we will use and Expression to build the Join clause
        public IDbQuery<TModel, TJoinTo> Join<TJoinTo>(JoinType joinType, Expression<Func<TModel, TJoinTo, object>> joinExpression) where TJoinTo : class, new()
        {
            return new DbQuery<TModel, TJoinTo>(_dbProvider, joinType, joinExpression);
        }

        public IDbQuery<TModel> Where(Expression<Func<TModel, object>> expression)
        {
            _whereExpressionVisitor = new WhereExpressionVisitor(_parameters).Visit(expression);
            _parameters = _whereExpressionVisitor.Parameters;

            if (string.IsNullOrEmpty(_whereClause))
            {
                _whereClause = string.Format(_dbProvider.Dialect.Where, _whereExpressionVisitor.WhereExpression);
            }
            else
            {
                _whereClause += " AND " + _whereExpressionVisitor.WhereExpression;
            }
            return this;
        }

        public int Count()
        {
            return _dbProvider.ExecuteScalar<int>( ToStringCount(), _parameters );
        }

        public IEnumerable<TModel> Select()
        {
            return _dbProvider.ExecuteReader(ToString(), _parameters, _dbMapper.BuildListFrom);
        }

        public IEnumerable<TModel> Select ( Func<IDbReader, IList<TModel>> mapperFunc )
        {
            return _dbProvider.ExecuteReader( ToString(), _parameters, mapperFunc );
        }

        public void Update(TModel model)
        {
            // TODO: Make the update only update specified feilds, this requires a lot more work though
            var mapper = new AutoDbMapper<TModel>();
            var dbFields = mapper.GetFieldNameList().Where(field => field != "ID").Select( field => string.Format( "[{0}] = @{0}", field ) ).ToList();

            var whereClause = GetExtendedWhereClause();
            var commandText = string.Format( _dbProvider.Dialect.Update, _tableName, string.Join( ",", dbFields ), whereClause );
            var parameters = _parameters.Union(mapper.BuildDbParametersFrom(model)).ToDictionary( pair => pair.Key, pair => pair.Value);
            _dbProvider.ExecuteNonQuery( commandText, parameters );
        }

        public void Delete()
        {
            _dbProvider.ExecuteNonQuery(ToStringDelete(), _parameters);
        }

        public void Truncate()
        {
            _dbProvider.ExecuteNonQuery( ToStringTruncate() );
        }

        public override string ToString()
        {
            return string.Format(_dbProvider.Dialect.SelectFrom, _tableName, GetExtendedWhereClause()).Trim();
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

        private string GetExtendedWhereClause()
        {
            return String.Join(" ", _whereClause, _orderByClause, _skipTake);
        }
    }

    public class DbQuery<TModel, TJoinTo> : IDbQuery<TModel, TJoinTo>where TModel : class, new() where TJoinTo : class, new()
    {
        private readonly IDbProvider _dbProvider;
        private readonly string _joinTableName;
        private readonly AutoDbMapper<TModel> _modelDbMapper;
        private readonly string _tableName;
        private string _joinExpression;
        private string _orderByClause;
        private string _whereClause;
        private string _skipTake;
        private Dictionary<string, object> _parameters;
        private OrderByExpressionVisitor _orderByExpressionVisitor;
        private WhereExpressionVisitor _whereExpressionVisitor;
        //private JoinExpressionVisitor _joinExpressionVisitor;

        // Called if its a ManyToMany join, we can grab the join conditions from out attributes, no need for expression
        public DbQuery(IDbProvider dbProvider, JoinType joinType)
        {
            _dbProvider = dbProvider;
            _modelDbMapper = new AutoDbMapper<TModel>();
            _tableName = typeof (TModel).GetTypeInfo().Name.BuildTableName();
            _joinTableName = typeof (TJoinTo).GetTypeInfo().Name.BuildTableName();
            _parameters = new Dictionary<string, object>();
            _joinExpression = BuildJoinExpression(joinType);
        }

        private string BuildJoinExpression(JoinType joinType)
        {
            if (joinType == JoinType.Inner)
            {
                return string.Format(_dbProvider.Dialect.InnerJoin, _tableName, "Id", _joinTableName, _tableName.Singularize() + "Id");
            }
            if (joinType == JoinType.Left)
            {
                return string.Format( _dbProvider.Dialect.LeftJoin, _tableName, "Id", _joinTableName, _tableName.Singularize() + "Id" );
            }
            if ( joinType == JoinType.ManyToMany)
            {
                var names = new[] { _tableName, _joinTableName };
                Array.Sort( names, StringComparer.CurrentCulture );
                var manyManyTableName = string.Join( "_", names );

                return string.Format( _dbProvider.Dialect.ManyToManyJoin, 
                    _tableName, "Id", manyManyTableName, _tableName.Singularize() + "Id", _joinTableName, _joinTableName.Singularize() + "Id");
            }
            throw new NotSupportedException( "The join type you selected is not yet supported." );
        }

        // Called for OneToOne and OneToMany joins, we need to look at the express to build Join clause
        public DbQuery(IDbProvider dbProvider, JoinType joinType, Expression<Func<TModel, TJoinTo, object>> joinExpression) : this(dbProvider, joinType)
        {
            throw new NotImplementedException();
            //_joinExpressionVisitor = new JoinExpressionVisitor().Visit(joinType, joinExpression);
        }

        public IDbQuery<TModel, TJoinTo> Where(Expression<Func<TModel, TJoinTo, object>> expression)
        {
            _whereExpressionVisitor = new WhereExpressionVisitor( _parameters ).Visit( expression );
            _parameters = _whereExpressionVisitor.Parameters;

            if ( string.IsNullOrEmpty( _whereClause ) )
            {
                _whereClause = string.Format( _dbProvider.Dialect.Where, _whereExpressionVisitor.WhereExpression );
            }
            else
            {
                _whereClause += " AND " + _whereExpressionVisitor.WhereExpression;
            }
            return this;
        }

        public IDbQuery<TModel, TJoinTo> OrderBy(Expression<Func<TModel, TJoinTo, object>> orderByExpression, OrderDirection direction)
        {
            _orderByExpressionVisitor = new OrderByExpressionVisitor().Visit( orderByExpression );
            _orderByClause = string.Format( _dbProvider.Dialect.OrderBy,
                                           _orderByExpressionVisitor.OrderByExpression,
                                           ( direction == OrderDirection.Ascending ) ? "ASC" : "DESC" );
            return this;
        }

        public IDbQuery<TModel, TJoinTo> SkipTake(int skip, int take)
        {
            _skipTake = string.Format( _dbProvider.Dialect.SkipTake, skip, take );
            return this;
        }

        public int Count()
        {
            return _dbProvider.ExecuteScalar<int>( ToStringCount(), _parameters );
        }

        public IEnumerable<TModel> Select ()
        {
            var commandText = ToString();
            return _dbProvider.ExecuteReader( commandText, _parameters, _modelDbMapper.BuildListFrom );
        }

        public IEnumerable<TModel> Select ( Func<IDbReader, IList<TModel>> mapperFunc )
        {
            return _dbProvider.ExecuteReader( ToString(), _parameters, mapperFunc );
        }

        public void Update(TModel model)
        {
            var mapper = new AutoDbMapper<TModel>();
            var dbFields = mapper.GetFieldNameList().Where( field => field != "ID" ).Select( field => string.Format( "[{0}] = @{0}", field ) ).ToList();

            var whereClause = GetExtendedWhereClause();
            var commandText = string.Format( _dbProvider.Dialect.UpdateJoin, _tableName, string.Join( ",", dbFields ), _joinExpression, whereClause );
            var parameters = _parameters.Union( mapper.BuildDbParametersFrom( model ) ).ToDictionary( pair => pair.Key, pair => pair.Value );
            _dbProvider.ExecuteNonQuery( commandText, parameters );
        }

        public void Delete()
        {
            _dbProvider.ExecuteNonQuery( ToStringDelete(), _parameters );
        }

        public override string ToString ()
        {
            return string.Format( _dbProvider.Dialect.SelectFromJoin, _tableName, _joinExpression, GetExtendedWhereClause() ).Trim();
        }

        public string ToStringCount ()
        {
            return string.Format( _dbProvider.Dialect.SelectCountFromJoin, _tableName, _joinExpression, GetExtendedWhereClause() ).Trim();
        }

        public string ToStringDelete ()
        {
            return string.Format( _dbProvider.Dialect.DeleteFromJoin, _tableName, _joinExpression, _whereClause );
        }

        private string GetExtendedWhereClause ()
        {
            return String.Join( " ", _whereClause, _orderByClause, _skipTake );
        }
    }
}