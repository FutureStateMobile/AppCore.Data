using System;
using FutureState.AppCore.Data.Tests.Helpers.Fixtures;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Unit
{
    [TestFixture]
    public class DbQueryTests : DbQueryTestBase
    {
        [Test,TestCaseSource(nameof(Repositories))]
        public void ShouldStripGenericMetaFromClassNameWhenBuildingQuery(IDbProvider dbProvider)
        {
            // setup
            const string expectedQuery = "SELECT [Foos].* FROM [Foos]";

            // execute
            var actualQuery = dbProvider.Query<Foo<Bar,Bar,Bar>>().ToString();

            // assert
            Assert.AreEqual(expectedQuery,actualQuery);
        }

        // NOTE: These tests have no way of working against a DELETE command.
        [Test, TestCaseSource( nameof(Repositories) )]
        public void ShouldBuildQueryForUsersGettingCount ( IDbProvider dbProvider )
        {
            // setup
            const string expectedQuery = @"SELECT COUNT([Authors].*) FROM [Authors] WHERE ( [Authors].[FirstName] LIKE @FirstName1 )";

            // execute
            var actualQuery = dbProvider.Query<AuthorModel>()
                                        .Where( u => u.FirstName.Contains("Bo") )
                                        .ToStringCount();

            // assert
            Assert.AreEqual( expectedQuery, actualQuery );
        }

        [Test, TestCaseSource(nameof(Repositories))]
        public void ShouldBuildQueryForUsersAndUtilizeSkipTake(IDbProvider dbProvider)
        {
            // setup
            var expectedQuery = string.Empty;
            switch (dbProvider.GetType().ToString())
            {
                case "FutureState.AppCore.Data.Sqlite.DbProvider":
                    expectedQuery = @"SELECT [Authors].* FROM [Authors] WHERE ( [Authors].[Id] = @Id1 ) ORDER BY [Authors].[Email] DESC LIMIT 10 OFFSET 20";
                    break;
                case "FutureState.AppCore.Data.SqlServer.DbProvider":
                    expectedQuery =
                        @"SELECT [Authors].* FROM [Authors] WHERE ( [Authors].[Id] = @Id1 ) ORDER BY [Authors].[Email] DESC OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY";
                    break;
            }

            var testGuid = Guid.NewGuid();

            // execute
            var actualQuery = dbProvider.Query<AuthorModel>()
                                        .Where(u => u.Id == testGuid)
                                        .OrderBy(u => u.Email, OrderDirection.Descending)
                                        .SkipTake(20, 10)
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }

        [Test, TestCaseSource(nameof(Repositories))]
        public void ShouldBuildQueryForUsersById(IDbProvider dbProvider)
        {
            // setup
            const string expectedQuery = @"SELECT [Authors].* FROM [Authors] WHERE ( [Authors].[Id] = @Id1 )";
            var testGuid = Guid.NewGuid();

            // execute
            var actualQuery = dbProvider.Query<AuthorModel>()
                                        .Where(u => u.Id == testGuid)
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }

        [Test, TestCaseSource(nameof(Repositories))]
        public void ShouldBuildQueryForUsersByNameAndNotName(IDbProvider dbProvider)
        {
            // setup
            const string expectedQuery =
                @"SELECT [Authors].* FROM [Authors] WHERE ( [Authors].[FirstName] LIKE @FirstName1 AND [Authors].[FirstName] <> @FirstName2 )";

            // execute
            var actualQuery = dbProvider.Query<AuthorModel>()
                                        .Where(u => u.FirstName.Contains("Bo") && u.FirstName != "Bob")
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }

        [Test, TestCaseSource( nameof(Repositories) )]
        public void ShouldBuildQueryForUsersByNameAndNotNameWithBrackets ( IDbProvider dbProvider )
        {
            // setup
            const string expectedQuery =
                @"SELECT [Authors].* FROM [Authors] WHERE ( [Authors].[FirstName] LIKE @FirstName1 OR [Authors].[FirstName] = @FirstName2 ) AND ( [Authors].[FirstName] <> @FirstName3 )";

            // execute
            var actualQuery = dbProvider.Query<AuthorModel>()
                                        .Where( u => u.FirstName.Contains( "Bo" ) || u.FirstName == "Kevin" )
                                        .Where( u => u.FirstName != "Bob" )
                                        .ToString();

            // assert
            Assert.AreEqual( expectedQuery, actualQuery );
        }

        [Test, TestCaseSource( nameof(Repositories) )]
        public void ShouldBuildQueryForUsersByNameNotNull ( IDbProvider dbProvider )
        {
            // setup
            const string expectedQuery = @"SELECT [Authors].* FROM [Authors] WHERE ( [Authors].[FirstName] IS NOT NULL )";

            // execute
            var actualQuery = dbProvider.Query<AuthorModel>()
                                        .Where( u => u.FirstName != null )
                                        .ToString();

            // assert
            Assert.AreEqual( expectedQuery, actualQuery );
        }


        [Test, TestCaseSource(nameof(Repositories))]
        public void ShouldBuildQueryWithOrderByAscClause(IDbProvider dbProvider)
        {
            // setup
            const string expectedQuery =
                @"SELECT [Authors].* FROM [Authors] WHERE ( [Authors].[FirstName] LIKE @FirstName1 ) ORDER BY [Authors].[Email] ASC";

            // execute
            var actualQuery = dbProvider.Query<AuthorModel>()
                                        .Where(u => u.FirstName.Contains("Bo"))
                                        .OrderBy(u => u.Email, OrderDirection.Ascending)
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }

        [Test, TestCaseSource(nameof(Repositories))]
        public void ShouldBuildQueryWithOrderByDescClause(IDbProvider dbProvider)
        {
            // setup
            const string expectedQuery =
                @"SELECT [Authors].* FROM [Authors] WHERE ( [Authors].[FirstName] LIKE @FirstName1 ) ORDER BY [Authors].[Email] DESC";

            // execute
            var actualQuery = dbProvider.Query<AuthorModel>()
                                        .Where(u => u.FirstName.Contains("Bo"))
                                        .OrderBy(u => u.Email, OrderDirection.Descending)
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }


        [Test, TestCaseSource(nameof(Repositories))]
        public void ShouldBuildQueryWithManyToOneRelationship(IDbProvider dbProvider)
        {
            // setup
            var id = Guid.NewGuid();
            const string expectedQuery =
                @"SELECT [Books].* FROM [Books] WHERE ( [Books].[PublisherId] = @PublisherId1 ) ORDER BY [Books].[Name] DESC";

            // execute
            var actualQuery = dbProvider.Query<BookModel>()
                                        .Where(b => b.Publisher.Id == id)
                                        .OrderBy(b => b.Name, OrderDirection.Descending)
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }
    }
}