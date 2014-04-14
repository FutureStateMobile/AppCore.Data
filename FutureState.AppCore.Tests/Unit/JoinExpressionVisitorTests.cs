using System;
using System.Linq.Expressions;
using FutureState.AppCore.Data;
using FutureState.AppCore.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Tests.Unit
{
    [TestFixture]
    public class JoinExpressionVisitorTests
    {
        public static JoinExpressionVisitor JoinExpression<TModel, TJoinTo> ( JoinType joinType, Expression<Func<TModel, TJoinTo, object>> joinExpression ) where TModel : class, new()
        {
            var visitor = new JoinExpressionVisitor().Visit(joinType, joinExpression );
            return visitor;
        }

        [Test, Ignore]
        public void ShouldBuildInnerJoinExpression ()
        {
            // Setup
            const string expectedString = "INNER JOIN Students.Id == Books.StudentId";
            var actualExpression = JoinExpression<StudentModel, BookModel>( JoinType.Inner, (s, b) => s.Id == b.StudentId );
            
            // Execute
            var actualString = actualExpression.JoinExpression;

            // Test
            Assert.AreEqual( expectedString, actualString );
        }
        
        [Test, Ignore]
        public void ShouldBuildLeftJoinExpression ()
        {
            // Setup
            const string expectedString = "LEFT JOIN Students.Id == Books.StudentId";
            var actualExpression = JoinExpression<StudentModel, BookModel>( JoinType.Left, ( s, b ) => s.Id == b.StudentId );

            // Execute
            var actualString = actualExpression.JoinExpression;

            // Test
            Assert.AreEqual( expectedString, actualString );
        }


    }
}