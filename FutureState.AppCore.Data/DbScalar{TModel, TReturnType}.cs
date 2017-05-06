using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data
{
    public class DbScalar<TModel, TReturnType> : IDbScalar<TModel, TReturnType> where TModel : class, new()
    {
        private readonly IDbProvider _dbProvider;
        private readonly string _propertyName;
        private readonly string _tableName;
        private Dictionary<string, object> _parameters;
        private string _whereClause;
        private WhereExpressionVisitor _whereExpressionVisitor;

        public DbScalar(IDbProvider dbProvider, Expression<Func<TModel, TReturnType>> propertyExpression)
        {
            _propertyName = GetPropertyName(propertyExpression);
            _dbProvider = dbProvider;
            _tableName = typeof(TModel).GetTypeInfo().Name.BuildTableName();
            _parameters = new Dictionary<string, object>();
        }

        public IDbScalar<TModel, TReturnType> Where(Expression<Func<TModel, object>> expression)
        {
            _whereExpressionVisitor = new WhereExpressionVisitor().Visit(expression);
            _whereClause = string.Format(_dbProvider.Dialect.Where, _whereExpressionVisitor.WhereExpression);
            _parameters = _whereExpressionVisitor.Parameters;

            return this;
        }

        public Task<TReturnType> MaxAsync() => _dbProvider.ExecuteScalarAsync<TReturnType>(ToStringMax(), _parameters);

        public Task<TReturnType> MinAsync() => _dbProvider.ExecuteScalarAsync<TReturnType>(ToStringMin(), _parameters);

        public Task<TReturnType> SumAsync() => _dbProvider.ExecuteScalarAsync<TReturnType>(ToStringSum(), _parameters);

        public TReturnType Max() => MaxAsync().Result;

        public TReturnType Min() => MinAsync().Result;

        public TReturnType Sum() => SumAsync().Result;

        public string ToStringMax() => string.Format(_dbProvider.Dialect.SelectMaxFrom, _tableName, _whereClause, _propertyName).Trim();

        public string ToStringMin() => string.Format(_dbProvider.Dialect.SelectMinFrom, _tableName, _whereClause, _propertyName).Trim();

        public string ToStringSum() => string.Format(_dbProvider.Dialect.SelectSumFrom, _tableName, _whereClause, _propertyName).Trim();

        private static MemberExpression GetMemberInfo(Expression method)
        {
            var lambda = method as LambdaExpression;

            if (lambda == null)
                throw new ArgumentNullException(nameof(method));

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
                memberExpr = ((UnaryExpression) lambda.Body).Operand as MemberExpression;
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
                memberExpr = lambda.Body as MemberExpression;

            if (memberExpr == null) 
                throw new ArgumentException("method");

            return memberExpr;
        }

        private static string GetPropertyName(Expression<Func<TModel, TReturnType>> propertyExpression) => GetMemberInfo(propertyExpression).Member.Name;
    }
}