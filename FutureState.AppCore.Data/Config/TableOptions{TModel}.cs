using System;
using System.Linq.Expressions;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data.Config
{
    public class TableOptions<TModel> where TModel : class, new()
    {
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
            var modelType = typeof(TModel);
            modelType.AddOrUpdatePrimaryKey(body.Member.Name);
            return this;
        }
    }
}