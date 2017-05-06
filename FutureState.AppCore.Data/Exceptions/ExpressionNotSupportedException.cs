using System;
using System.Linq.Expressions;

namespace FutureState.AppCore.Data.Exceptions
{
    public class ExpressionNotSupportedException : Exception
    {
        private const string _expressionNotSupported = "Unhandled expression type: '{0}'";

        public ExpressionNotSupportedException(Expression expression)
            : base(string.Format(_expressionNotSupported, expression.NodeType))
        {
        }
    }
}