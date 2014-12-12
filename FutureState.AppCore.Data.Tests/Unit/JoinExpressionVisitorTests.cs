using System;
using System.Linq.Expressions;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Unit
{
    [TestFixture]
    public class JoinExpressionVisitorTests
    {
        public static JoinExpressionVisitor JoinExpression<TModel, TJoinTo>(Expression<Func<TModel, TJoinTo, object>> joinExpression) where TModel : class, new()
        {
            JoinExpressionVisitor visitor = new JoinExpressionVisitor().Visit(joinExpression);
            return visitor;
        }

        [Test]
        public void ShouldBuildInnerJoinExpression()
        {
            // Setup
            const string expectedString = "[Books].[PublisherId] = [Publishers].[Id]";
            JoinExpressionVisitor actualExpression = JoinExpression<BookModel, PublisherModel>((b, p) => b.Publisher.Id == p.Id);

            // Execute
            string actualString = actualExpression.JoinExpression;

            // Test
            Assert.AreEqual(expectedString, actualString);
        }

        [Test]
        public void ShouldBuildLeftJoinExpression()
        {
            // Setup
            const string expectedString = "[Publishers].[Id] = [Books].[PublisherId]";
            JoinExpressionVisitor actualExpression = JoinExpression<BookModel, PublisherModel>((b, p) => p.Id == b.Publisher.Id);

            // Execute
            string actualString = actualExpression.JoinExpression;

            // Test
            Assert.AreEqual(expectedString, actualString);
        }
    }
}