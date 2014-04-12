using System;
using System.Linq.Expressions;
using System.Text;

namespace FutureState.AppCore.Data
{
    public class JoinExpressionVisitor
    {
        private readonly StringBuilder _strings;

        public JoinExpressionVisitor()
        {
            _strings = new StringBuilder();
        }

        public string JoinExpression
        {
            get { return _strings.ToString().Trim(); }
        }

        public JoinExpressionVisitor Visit(JoinType joinType, Expression joinExpression)
        {
            throw new NotImplementedException();
        }
    }
}