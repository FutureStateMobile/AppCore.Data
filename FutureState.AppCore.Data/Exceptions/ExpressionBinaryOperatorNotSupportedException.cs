using System;
using System.Linq.Expressions;

namespace FutureState.AppCore.Data.Exceptions
{
    internal class ExpressionBinaryOperatorNotSupportedException : Exception
    {
        private const string OperatorNotSupported = "The binary operator '{0}' is not supported.";

        public ExpressionBinaryOperatorNotSupportedException(BinaryExpression expression)
            : base(string.Format(OperatorNotSupported, expression.NodeType))
        {
        }
    }
}