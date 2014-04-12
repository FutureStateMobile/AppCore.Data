using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FutureState.AppCore.Data.Exceptions;

namespace FutureState.AppCore.Data
{
    public class WhereExpressionVisitor
    {
        private readonly StringBuilder _strings;
        public Dictionary<string, object> Parameters;

        public WhereExpressionVisitor()
        {
            Parameters = new Dictionary<string, object>();
            _strings = new StringBuilder();
        }

        public string WhereExpression
        {
            get { return _strings.ToString().Trim(); }
        }

        public WhereExpressionVisitor Visit(Expression expression)
        {
            Visit(expression, null);
            return this;
        }

        private void Visit(Expression expression, Expression left)
        {
            if (expression == null)
                throw new NullReferenceException();

            switch (expression.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    VisitUnary((UnaryExpression) expression);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    VisitBinary((BinaryExpression) expression);
                    break;
                case ExpressionType.TypeIs:
                    VisitTypeIs((TypeBinaryExpression) expression);
                    break;
                case ExpressionType.Conditional:
                    VisitConditional((ConditionalExpression) expression);
                    break;
                case ExpressionType.Constant:
                    VisitConstant((ConstantExpression) expression, (MemberExpression) left);
                    break;
                case ExpressionType.Parameter:
                    VisitParameter((ParameterExpression) expression);
                    break;
                case ExpressionType.MemberAccess:
                    VisitMemberAccess((MemberExpression) expression, (MemberExpression) left);
                    break;
                case ExpressionType.Call:
                    VisitMethodCall((MethodCallExpression) expression);
                    break;
                case ExpressionType.Lambda:
                    VisitLambda((LambdaExpression) expression);
                    break;
                case ExpressionType.New:
                    VisitNew((NewExpression) expression);
                    break;
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    VisitNewArray((NewArrayExpression) expression);
                    break;
                case ExpressionType.Invoke:
                    VisitInvocation((InvocationExpression) expression);
                    break;
                case ExpressionType.MemberInit:
                    VisitMemberInit((MemberInitExpression) expression);
                    break;
                case ExpressionType.ListInit:
                    VisitListInit((ListInitExpression) expression);
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

        private void VisitUnary(UnaryExpression expression)
        {
            Expression unary = expression.Operand;
            Visit(unary);
        }

        private void VisitBinary(BinaryExpression expression)
        {
            // AddExpressionParameter(expression);

            Visit(expression.Left);
            GetBinaryOperator(expression);

            if (expression.Right is ConstantExpression || expression.Right is MemberExpression)
            {
                Visit(expression.Right, expression.Left);
                return;
            }

            Visit(expression.Right);
        }

        private void VisitTypeIs(TypeBinaryExpression expression)
        {
            throw new NotImplementedException();
        }

        private void VisitConditional(ConditionalExpression expression)
        {
            throw new NotImplementedException();
        }

        private void VisitParameter(ParameterExpression expression)
        {
            throw new NotImplementedException();
        }

        private void VisitNew(NewExpression expression)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<Expression> VisitExpressionList(ReadOnlyCollection<Expression> arguments)
        {
            throw new NotImplementedException();
        }

        private void VisitNewArray(NewArrayExpression expression)
        {
            throw new NotImplementedException();
        }

        private void VisitInvocation(InvocationExpression expression)
        {
            throw new NotImplementedException();
        }

        private void VisitMemberInit(MemberInitExpression expression)
        {
            throw new NotImplementedException();
        }

        private void VisitListInit(ListInitExpression expression)
        {
            throw new NotImplementedException();
        }

        private void VisitMethodCall(MethodCallExpression expression)
        {
            MethodCallExpression exp = expression;

            string commandText = GetMethodCallFormat(expression);
            string parameterFormat = GetParameterFormat(expression);
            var memberExpression = (MemberExpression) exp.Object;
            var constantExpression = exp.Arguments[0] as ConstantExpression;
            object expressionValue;

            if (memberExpression == null)
                throw new ArgumentException();

            if (constantExpression == null)
                expressionValue = GetMemberExpressionValue(exp.Arguments[0] as MemberExpression);
            else
                expressionValue = constantExpression.Value;

            string key = memberExpression.Member.Name;
            string value = string.Format(parameterFormat, expressionValue);

            // Format the Method string to include the Key/Parameter
            int number = AddParameter("@" + key, value, 1);
            _strings.Append(string.Format(commandText, key, String.Format(key + "{0}", number)));
            _strings.Append(" ");

            // add parameter value
            //Parameters.Add("@" + key, value);
        }

        private void VisitConstant(ConstantExpression expression, MemberExpression left)
        {
            string key = "@" + left.Member.Name;
            // add the parameter value
            int number = AddParameter(key, expression.Value, 1);
            // add the string parameter
            _strings.Append(String.Format(key + "{0}", number));
            _strings.Append(" ");
        }

        private int AddParameter(string key, object value, int times)
        {
            int newTimes = times;
            string newKey = String.Format(key + "{0}", times);
            if (Parameters.ContainsKey(newKey))
            {
                newTimes = AddParameter(key, value, ++times);
            }
            else
            {
                Parameters.Add(newKey, value);
            }

            return newTimes;
        }

        private void VisitMemberAccess(MemberExpression expression, MemberExpression left)
        {
            // To preserve Case between key/value pairs, we always want to use the LEFT side of the expression.
            // therefore, if left is null, then expression is actually left. 
            // Doing this ensures that our `key` matches between parameter names and database fields
            string key = left != null ? left.Member.Name : expression.Member.Name;

            // If the NodeType is a `Parameter`, we want to add the key as a DB Field name to our string collection
            // Otherwise, we want to add the key as a DB Parameter to our string collection
            if (expression.Expression.NodeType.ToString() == "Parameter")
            {
                _strings.Append(string.Format("[{0}]", key));
                _strings.Append(" ");
            }
            else
            {
                int paramNo = AddParameter("@" + key, GetMemberExpressionValue(expression), 1);

                _strings.Append(string.Format("@{0}", String.Format(key + "{0}", paramNo)));
                _strings.Append(" ");
                //Parameters.Add("@" + key, GetMemberExpressionValue(expression));
            }
        }

        private object GetMemberExpressionValue(MemberExpression memberExpression)
        {
            // If the key is being added as a DB Parameter, then we have to also add the Parameter key/value pair to the collection
            // Because we're working off of Model Objects that should only contain Properties or Fields,
            // there should only be two options. PropertyInfo or FieldInfo... let's extract the VALUE accordingly
            if ((memberExpression.Member as PropertyInfo) != null)
            {
                var exp = (MemberExpression) memberExpression.Expression;
                var constantExpression = (ConstantExpression) exp.Expression;
                object fieldInfoValue = ((FieldInfo) exp.Member).GetValue(constantExpression.Value);
                return ((PropertyInfo) memberExpression.Member).GetValue(fieldInfoValue, null);
            }
            if ((memberExpression.Member as FieldInfo) != null)
            {
                var fieldInfo = memberExpression.Member as FieldInfo;
                var constantExpression = memberExpression.Expression as ConstantExpression;
                if (fieldInfo != null & constantExpression != null)
                {
                    return fieldInfo.GetValue(constantExpression.Value);
                }
            }

            throw new InvalidMemberException();
        }

        private void GetBinaryOperator(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                    _strings.Append("=");
                    _strings.Append(" ");
                    break;
                case ExpressionType.NotEqual:
                    _strings.Append("<>");
                    _strings.Append(" ");
                    break;
                case ExpressionType.OrElse:
                    _strings.Append("OR");
                    _strings.Append(" ");
                    break;
                case ExpressionType.AndAlso:
                    _strings.Append("AND");
                    _strings.Append(" ");
                    break;
                case ExpressionType.LessThan:
                    _strings.Append("<");
                    _strings.Append(" ");
                    break;
                case ExpressionType.GreaterThan:
                    _strings.Append(">");
                    _strings.Append(" ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _strings.Append(">=");
                    _strings.Append(" ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _strings.Append("<=");
                    _strings.Append(" ");
                    break;
                default:
                    throw new ExpressionBinaryOperatorNotSupportedException(expression);
            }
        }

        private static string GetParameterFormat(MethodCallExpression expression)
        {
            switch (expression.Method.Name)
            {
                case "Contains":
                    return "%{0}%";
                case "StartsWith":
                    return "{0}%";
                case "EndsWith":
                    return "%{0}";
                case "Equals":
                    return "{0}";
                default:
                    throw new ExpressionMethodCallNotSupportedException(expression);
            }
        }

        private static string GetMethodCallFormat(MethodCallExpression expression)
        {
            switch (expression.Method.Name)
            {
                case "EndsWith":
                case "StartsWith":
                case "Contains":
                    return "[{0}] LIKE @{1}";
                case "Equals":
                    return "[{0}] = @{1}";
                default:
                    throw new ExpressionMethodCallNotSupportedException(expression);
            }
        }
    }
}