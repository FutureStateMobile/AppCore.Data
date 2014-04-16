using FutureState.AppCore.Data.Exceptions;
using System;
using System.Linq.Expressions;
using System.Text;

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
                    VisitLambda((LambdaExpression)expression);
                    break;
                case ExpressionType.MemberAccess:
                    VisitMemberAccess((MemberExpression)expression);
                    break;
                default:
                    throw new ExpressionNotSupportedException(expression);
            }
        }
        private void VisitLambda(LambdaExpression expression)
        {
            Expression lambda = expression.Body;
            Visit(lambda);
        }
        private void VisitMemberAccess(MemberExpression expression)
        {

            _strings.AppendFormat("[{0}]", expression.Member.Name);
        }
    }
}