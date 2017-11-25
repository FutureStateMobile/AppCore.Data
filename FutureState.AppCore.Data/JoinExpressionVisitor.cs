using System;
using System.Linq.Expressions;
using System.Text;
using FutureState.AppCore.Data.Exceptions;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data
{
    public class JoinExpressionVisitor
    {
        private readonly StringBuilder _strings;

        public JoinExpressionVisitor()
        {
            _strings = new StringBuilder();
        }

        public string JoinExpression => _strings.ToString().Trim();

        public JoinExpressionVisitor Visit(Expression expression)
        {
            VisitExpression(expression);
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
                    return;
                case ExpressionType.MemberAccess:
                    VisitMemberAccess((MemberExpression) expression);
                    return;
                case ExpressionType.Convert:
                    VisitUnary((UnaryExpression)expression);
                    return;
                case ExpressionType.Equal:
                    VisitBinary((BinaryExpression)expression);
                    return;
                default:
                    throw new ExpressionNotSupportedException(expression);
            }
        }

        private void VisitLambda(LambdaExpression expression)
        {
            Visit(expression.Body);
        }

        private void VisitMemberAccess(MemberExpression expression)
        {
            var tableName = BuildTableName(expression);
            var columnName = BuildColumnName(expression);
            _strings.AppendFormat("[{0}].[{1}]", tableName, columnName);
        }

        private void VisitUnary(UnaryExpression expression)
        {
            Visit(expression.Operand);
        }


        protected virtual Expression VisitBinary(BinaryExpression binaryExpression)
        {
            if (binaryExpression.NodeType != ExpressionType.Equal)
            {
                throw new ExpressionNotSupportedException(binaryExpression);
            }

            VisitExpression(binaryExpression.Left);
            _strings.Append(" = ");
            VisitExpression(binaryExpression.Right);

            return binaryExpression;
        }

        private string BuildTableName(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            return memberExpression != null ? 
                BuildTableName(memberExpression.Expression) : 
                expression.Type.Name.BuildTableName();
        }

        private string BuildColumnName(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            return memberExpression != null ? 
                BuildColumnName(memberExpression.Expression) + memberExpression.Member.Name : 
                "";
        }

    }
}