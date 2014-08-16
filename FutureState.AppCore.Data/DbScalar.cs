using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FutureState.AppCore.Data.Helpers;

namespace FutureState.AppCore.Data
{
    public class DbScalar<TModel, TReturnType> : IDbScalar<TModel, TReturnType> where TModel : class, new()
    {
        private readonly IDbProvider _dbProvider;
        private Dictionary<string, object> _parameters;
        private string _whereClause;
        private readonly string _propertyName;
        private readonly string _tableName;
        private WhereExpressionVisitor _whereExpressionVisitor;

        public DbScalar ( IDbProvider dbProvider, Expression<Func<TModel, TReturnType>> propertyExpression )
        {
            _propertyName = GetPropertyName( propertyExpression );
            _dbProvider = dbProvider;
            _tableName = GetTableName(typeof (TModel).GetTypeInfo());
            _parameters = new Dictionary<string, object>();
        }

        public IDbScalar<TModel, TReturnType> Where ( Expression<Func<TModel, object>> expression )
        {
            _whereExpressionVisitor = new WhereExpressionVisitor().Visit( expression );
            _whereClause = string.Format( _dbProvider.Dialect.Where, _whereExpressionVisitor.WhereExpression );
            _parameters = _whereExpressionVisitor.Parameters;

            return this;
        }

        public TReturnType Max()
        {
            return _dbProvider.ExecuteScalar<TReturnType>( ToStringMax(), _parameters );
        }

        public TReturnType Min()
        {
            return _dbProvider.ExecuteScalar<TReturnType>( ToStringMin(), _parameters );
        }

        public TReturnType Sum()
        {
            return _dbProvider.ExecuteScalar<TReturnType>( ToStringSum(), _parameters );
        }

        public string ToStringMax()
        {
            return string.Format( _dbProvider.Dialect.SelectMaxFrom, _tableName, _whereClause, _propertyName ).Trim();
        }
        
        public string ToStringMin ()
        {
            return string.Format( _dbProvider.Dialect.SelectMinFrom, _tableName, _whereClause, _propertyName ).Trim();
        }

        public string ToStringSum ()
        {
            return string.Format( _dbProvider.Dialect.SelectSumFrom, _tableName, _whereClause, _propertyName ).Trim();
        }

        private static string GetTableName ( MemberInfo type )
        {
            return type.Name.Replace( "Model", "" ).Pluralize();
        }

        private static string GetPropertyName( Expression<Func<TModel, TReturnType>> propertyExpression )
        {
            var expression = GetMemberInfo( propertyExpression );
            return expression.Member.Name;
        }

        private static MemberExpression GetMemberInfo ( Expression method )
        {
            var lambda = method as LambdaExpression;
            if ( lambda == null )
                throw new ArgumentNullException( "method" );

            MemberExpression memberExpr = null;

            if ( lambda.Body.NodeType == ExpressionType.Convert )
            {
                memberExpr =
                    ( (UnaryExpression)lambda.Body ).Operand as MemberExpression;
            }
            else if ( lambda.Body.NodeType == ExpressionType.MemberAccess )
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if ( memberExpr == null )
                throw new ArgumentException( "method" );

            return memberExpr;
        }

    }
}