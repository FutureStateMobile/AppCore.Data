using System;
using System.Linq.Expressions;
using FutureState.AppCore.Data.Exceptions;

namespace FutureState.AppCore.Data.Config
{
    public class TableOptions<TModel> where TModel : class, new()
    {
        private const int _maxMemberExpressionTries = 3;
        private readonly DbProviderBase _dbProvider;

        public TableOptions(IDbProvider dbProvider)
        {
            _dbProvider = (DbProviderBase) dbProvider;
        }

        /// <summary>
        ///     Tells the DbProvider what the primary key is for <see cref="TModel" />
        /// </summary>
        public TableOptions<TModel> SetPrimaryKey(Expression<Func<TModel, object>> func)
        {
            var body = func.Body as MemberExpression;
            var count = 0;
            while (body == null)
            {
                var ubody = (UnaryExpression) func.Body;
                body = ubody.Operand as MemberExpression;
                if (count == _maxMemberExpressionTries)
                    throw new ExpressionNotSupportedException(func);
                count++;
            }
            _dbProvider.AddOrUpdatePrimaryKey(typeof(TModel), body.Member.Name);
            return this;
        }
    }
}