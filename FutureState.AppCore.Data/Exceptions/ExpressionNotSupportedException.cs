using System;
using System.Linq.Expressions;

namespace FutureState.AppCore.Data.Exceptions
{
    public class ExpressionNotSupportedException : Exception
    {
        private const string ExpressionNotSupported = "Unhandled expression type: '{0}'";

        public ExpressionNotSupportedException(Expression expression)
            : base(string.Format(ExpressionNotSupported, expression.NodeType))
        {
        }
    }
}