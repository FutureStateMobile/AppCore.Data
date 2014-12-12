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

        public String JoinExpression
        {
            get { return _strings.ToString().Trim(); }
        }

        public JoinExpressionVisitor Visit(Expression expression)
        {
            VisitExpression(expression);
            return this;
        }

        private Expression VisitExpression(Expression expression)
        {
            if (expression == null)
                throw new NullReferenceException();

            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression) expression);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression) expression);
                case ExpressionType.Convert:
                    return VisitUnary((UnaryExpression)expression);
                case ExpressionType.Equal:
                    return VisitBinary((BinaryExpression)expression);
                default:
                    throw new ExpressionNotSupportedException(expression);
            }
        }

        private Expression VisitLambda(LambdaExpression expression)
        {
            Visit(expression.Body);
            return expression;
        }

        private Expression VisitMemberAccess(MemberExpression expression)
        {
            var tableName = BuildTableName(expression);
            var columnName = BuildColumnName(expression);
            _strings.AppendFormat("[{0}].[{1}]", tableName, columnName);
            return expression;
        }

        private Expression VisitUnary(UnaryExpression expression)
        {
            Visit(expression.Operand);
            return expression;
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
            if (memberExpression != null)
            {
                return BuildTableName(memberExpression.Expression);
            }
            return expression.Type.Name.BuildTableName();
        }

        private string BuildColumnName(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                return BuildColumnName(memberExpression.Expression) + memberExpression.Member.Name;
            }
            return "";
        }

    }
}