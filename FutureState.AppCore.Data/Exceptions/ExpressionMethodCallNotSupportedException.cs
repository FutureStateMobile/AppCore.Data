using System;
using System.Linq.Expressions;

namespace FutureState.AppCore.Data.Exceptions
{
    public class ExpressionMethodCallNotSupportedException : Exception
    {
        private const string _methodNotSupported = "The method call '{0}' is not supported.";

        public ExpressionMethodCallNotSupportedException(MethodCallExpression expression)
            : base(string.Format(_methodNotSupported, expression.Method.Name))
        {
        }
    }
}