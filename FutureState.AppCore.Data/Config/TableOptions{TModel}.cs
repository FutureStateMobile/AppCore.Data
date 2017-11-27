using System;
using System.Linq.Expressions;

namespace FutureState.AppCore.Data.Config
{
    public class TableOptions<TModel> where TModel : class, new()
    {
        private readonly DbProviderBase _dbProvider;

        public TableOptions(IDbProvider dbProvider)
        {
            _dbProvider = (DbProviderBase) dbProvider;
        }

        /// <summary>
        ///     Tells the DbProvider what the PrimaryKey is for <see cref="TModel" />
        /// </summary>
        public TableOptions<TModel> SetPrimaryKey(Expression<Func<TModel, object>> func)
        {
            var body = func.Body as MemberExpression;
            if (body == null)
            {
                var ubody = (UnaryExpression) func.Body;
                body = (MemberExpression)ubody.Operand;
            }
            _dbProvider.AddOrUpdatePrimaryKey(typeof(TModel), body.Member.Name);
            return this;
        }
    }
}