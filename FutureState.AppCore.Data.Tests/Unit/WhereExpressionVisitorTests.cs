using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Unit
{
    [TestFixture]
    public class WhereExpressionVisitorTests
    {
        public static WhereExpressionVisitor TestExpression<TModel>(Expression<Func<TModel, object>> expression)
            where TModel : class, new()
        {
            var visitor = new WhereExpressionVisitor().Visit(expression);
            return visitor;
        }

        private delegate void TestMethod(string searchString);

        public StudentModel TestProperty { get; set; }

        [Test]
        public void ShouldBuildAWhereClauseWithAnOr()
        {
            // Setup
            const string expectedString = "[FirstName] LIKE @FirstName1 OR [LastName] LIKE @LastName1";
            const string name = "ues";
            var expectedParameters = new Dictionary<string, object>
                {
                    {"@FirstName1", "%" + name + "%"},
                    {"@LastName1", "%" + name + "%"}
                };
            var actualString = "";
            var actualParameters = new Dictionary<string, object>();

            // Put the call into an anonymous so we can call it in a similar way to how it happens as part of a repository

            TestMethod testMethod = delegate(string item)
                {
                    var actualExpression =
                        TestExpression<StudentModel>(u => u.FirstName.Contains(item) || u.LastName.Contains(item));
                    actualString = actualExpression.WhereExpression;
                    actualParameters = actualExpression.Parameters;
                };

            // Execute
            testMethod(name);

            // Test
            Assert.AreEqual(expectedString, actualString);
            CollectionAssert.AreEquivalent(expectedParameters, actualParameters);
        }

        [Test]
        public void ShouldBuildAWhereClauseWithSameFieldUsedMoreThanOnce()
        {
            // Setup
            const string expectedString = "[Email] <> @Email1 AND [Email] <> @Email2";
            var expectedParameters = new Dictionary<string, object> {{"@Email1", null}, {"@Email2", ""}};
            var actualExpression = TestExpression<StudentModel>(u => u.Email != "thing" && u.Email != "");

            // Execute
            var actualString = actualExpression.WhereExpression;
            var actualParameters = actualExpression.Parameters;

            // Test
            Assert.AreEqual(expectedString, actualString);
            CollectionAssert.AreEquivalent(expectedParameters, actualParameters);
        }

        [Test]
        public void ShouldVisitExpressionByDateObject()
        {
            // Setup
            var date = DateTime.UtcNow;
            const string expectedString = "[CreatedDate] = @CreatedDate1";
            var expectedParameters = new Dictionary<string, object> {{"@CreatedDate1", date}};

            // Execute
            var actualExpression = TestExpression<StudentModel>(u => u.CreatedDate == date);
            var actualParameters = actualExpression.Parameters;
            var actualString = actualExpression.WhereExpression;

            // Test
            Assert.AreEqual(expectedString, actualString);
            CollectionAssert.AreEquivalent(expectedParameters, actualParameters);
        }

        [Test]
        public void ShouldVisitExpressionByGuidObject()
        {
            // Setup
            var id = new Guid("CCAF57D9-88A4-4DCD-87C7-DB875E0D4E66");
            const string expectedString = "[Id] = @Id1";
            var expectedParameters = new Dictionary<string, object> {{"@Id1", id}};

            // Execute
            var actualExpression = TestExpression<StudentModel>(u => u.Id == id);
            var actualParameters = actualExpression.Parameters;
            var actualString = actualExpression.WhereExpression;

            // Test
            Assert.AreEqual(expectedString, actualString);
            CollectionAssert.AreEquivalent(expectedParameters, actualParameters);
        }

        [Test]
        public void ShouldVisitExpressionByStringLiteral()
        {
            // Setup
            const string expectedString = "[Email] = @Email1";
            var expectedParameters = new Dictionary<string, object> {{"@Email1", "john@doe.com"}};

            // Execute
            var actualExpression = TestExpression<StudentModel>(u => u.Email == "john@doe.com");
            var actualParameters = actualExpression.Parameters;
            var actualString = actualExpression.WhereExpression;

            // Assert
            Assert.AreEqual(expectedString, actualString);
            CollectionAssert.AreEquivalent(expectedParameters, actualParameters);
        }

        [Test]
        public void ShouldVisitExpressionByStringMethod()
        {
            // Setup
            const string email = "john@doe.com";
            const string expectedString = "[Email] = @Email1";
            var expectedParameters = new Dictionary<string, object> {{"@Email1", email}};

            // Execute
            var actualExpression = TestExpression<StudentModel>(u => u.Email.Equals(email));
            var actualParameters = actualExpression.Parameters;
            var actualString = actualExpression.WhereExpression;

            // Test
            Assert.AreEqual(expectedString, actualString);
            CollectionAssert.AreEquivalent(expectedParameters, actualParameters);
        }

        [Test]
        public void ShouldVisitExpressionByStringObject()
        {
            // Setup
            var expectedUser = new StudentModel {Email = "john@doe.com"};

            const string expectedString = "[Email] = @Email1";
            var expectedParameters = new Dictionary<string, object> {{"@Email1", expectedUser.Email}};

            // Execute
            var actualExpression = TestExpression<StudentModel>(u => u.Email == expectedUser.Email);
            var actualParameters = actualExpression.Parameters;
            var actualString = actualExpression.WhereExpression;

            // Assert
            Assert.AreEqual(expectedString, actualString);
            CollectionAssert.AreEquivalent(expectedParameters, actualParameters);
        }

        [Test]
        public void ShouldVisitExpressionOnPublicProperties()
        {
            // Setup
            TestProperty = new StudentModel
                {
                    Id = new Guid("91191CA1-74D7-4751-B6AA-11F060403FBA"),
                    FirstName = "Bob",
                    LastName = "Smith"
                };

            const string email = "john@doe.com";
            const string expectedString = "[Email] = @Email1 AND [FirstName] = @FirstName1";
            var expectedParameters = new Dictionary<string, object>
                {
                    {"@Email1", email},
                    {"@FirstName1", "Bob"}
                };

            // Execute
            var actualExpression =
                TestExpression<StudentModel>(u => u.Email.Equals(email) && u.FirstName == TestProperty.FirstName);
            var actualParameters = actualExpression.Parameters;
            var actualString = actualExpression.WhereExpression;

            // Test
            Assert.AreEqual(expectedString, actualString);
            CollectionAssert.AreEquivalent(expectedParameters, actualParameters);
        }
    }
}