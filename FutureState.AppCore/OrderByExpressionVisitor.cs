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
            throw new NotImplementedException();
        }
    }
}