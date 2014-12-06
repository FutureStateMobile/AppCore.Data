using System;
using System.Linq.Expressions;
using System.Text;
using FutureState.AppCore.Data.Exceptions;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data
{
    public class OrderByExpressionVisitor
    {
        private readonly StringBuilder _strings;

        public OrderByExpressionVisitor()
        {
            _strings = new StringBuilder();
        }

        public String OrderByExpression
        {
            get { return _strings.ToString().Trim(); }
        }

        public OrderByExpressionVisitor Visit(Expression orderByExpression)
        {
            VisitExpression(orderByExpression);
            return this;
        }

        private void VisitExpression(Expression expression)
        {
            if (expression == null)
                throw new NullReferenceException();

            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    VisitLambda((LambdaExpression) expression);
                    break;
                case ExpressionType.MemberAccess:
                    VisitMemberAccess((MemberExpression) expression);
                    break;
                case ExpressionType.Convert:
                    var exp = expression as UnaryExpression;
                    if (exp != null)
                    {
                        VisitMemberAccess( (MemberExpression)exp.Operand );
                    }
                    break;
                default:
                    throw new ExpressionNotSupportedException(expression);
            }
        }

        private void VisitLambda(LambdaExpression expression)
        {
            var lambda = expression.Body;
            Visit(lambda);
        }

        private void VisitMemberAccess(MemberExpression expression)
        {
            var tableName = expression.Expression.Type.Name.BuildTableName();
            _strings.AppendFormat( "[{0}].[{1}]", tableName, expression.Member.Name );
        }
    }
}