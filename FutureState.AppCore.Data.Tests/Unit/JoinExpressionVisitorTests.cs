using System;
using System.Linq.Expressions;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Unit
{
    [TestFixture]
    public class JoinExpressionVisitorTests
    {
        public static JoinExpressionVisitor JoinExpression<TModel, TJoinTo>(JoinType joinType, Expression<Func<TModel, TJoinTo, object>> joinExpression)
            where TModel : class, new()
        {
            JoinExpressionVisitor visitor = new JoinExpressionVisitor().Visit(joinType, joinExpression);
            return visitor;
        }

        [Test, Ignore]
        public void ShouldBuildInnerJoinExpression()
        {
            // Setup
            const string expectedString = "INNER JOIN Authors.Id == Books.AuthorId";
            JoinExpressionVisitor actualExpression = JoinExpression<AuthorModel, BookModel>(JoinType.Inner, (s, b) => s.Id == b.Author.Id);

            // Execute
            string actualString = actualExpression.JoinExpression;

            // Test
            Assert.AreEqual(expectedString, actualString);
        }

        [Test, Ignore]
        public void ShouldBuildLeftJoinExpression()
        {
            // Setup
            const string expectedString = "LEFT JOIN Authors.Id == Books.AuthorId";
            JoinExpressionVisitor actualExpression = JoinExpression<AuthorModel, BookModel>(JoinType.Left, (s, b) => s.Id == b.Author.Id);

            // Execute
            string actualString = actualExpression.JoinExpression;

            // Test
            Assert.AreEqual(expectedString, actualString);
        }
    }
}