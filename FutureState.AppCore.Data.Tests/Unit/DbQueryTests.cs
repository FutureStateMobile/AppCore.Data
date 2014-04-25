using System;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Unit
{
    [TestFixture]
    public class DbQueryTests : DbQueryTestBase
    {
        // NOTE: These tests have no way of working against a DELETE command.

        [Test, TestCaseSource("Repositories")]
        public void ShouldBuildQueryForUsersAndUtilizeSkipTake(IDbProvider dbProvider)
        {
            // setup
            var expectedQuery = string.Empty;
            switch (dbProvider.GetType().ToString())
            {
                case "FutureState.AppCore.Data.Sqlite.DbProvider":
                    expectedQuery = @"SELECT Students.* FROM Students WHERE [Id] = @Id1  LIMIT 10 OFFSET 20";
                    break;
                case "FutureState.AppCore.Data.SqlServer.DbProvider":
                    expectedQuery =
                        @"SELECT Students.* FROM Students WHERE [Id] = @Id1  OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY";
                    break;
            }

            var testGuid = Guid.NewGuid();

            // execute
            var actualQuery = dbProvider.Query<StudentModel>()
                                        .Where(u => u.Id == testGuid)
                                        .SkipTake(20, 10)
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }

        [Test, TestCaseSource("Repositories")]
        public void ShouldBuildQueryForUsersById(IDbProvider dbProvider)
        {
            // setup
            const string expectedQuery = @"SELECT Students.* FROM Students WHERE [Id] = @Id1";
            var testGuid = Guid.NewGuid();

            // execute
            var actualQuery = dbProvider.Query<StudentModel>()
                                        .Where(u => u.Id == testGuid)
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }


        [Test, TestCaseSource("Repositories")]
        public void ShouldBuildQueryForUsersByNameAndNotName(IDbProvider dbProvider)
        {
            // setup
            const string expectedQuery =
                @"SELECT Students.* FROM Students WHERE [FirstName] LIKE @FirstName1 AND [FirstName] <> @FirstName2";

            // execute
            var actualQuery = dbProvider.Query<StudentModel>()
                                        .Where(u => u.FirstName.Contains("Bo") && u.FirstName != "Bob")
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }

        [Test, TestCaseSource("Repositories")]
        public void ShouldBuildQueryWithOrderByAscClause(IDbProvider dbProvider)
        {
            // TODO: Joe to implement
            // setup
            const string expectedQuery =
                @"SELECT Students.* FROM Students WHERE [FirstName] LIKE @FirstName1 ORDER BY [Email] ASC";

            // execute
            var actualQuery = dbProvider.Query<StudentModel>()
                                        .Where(u => u.FirstName.Contains("Bo"))
                                        .OrderBy(u => u.Email, OrderDirection.Ascending)
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }

        [Test, TestCaseSource("Repositories")]
        public void ShouldBuildQueryWithOrderByDescClause(IDbProvider dbProvider)
        {
            // TODO: Joe to implement
            // setup
            const string expectedQuery =
                @"SELECT Students.* FROM Students WHERE [FirstName] LIKE @FirstName1 ORDER BY [Email] DESC";

            // execute
            var actualQuery = dbProvider.Query<StudentModel>()
                                        .Where(u => u.FirstName.Contains("Bo"))
                                        .OrderBy(u => u.Email, OrderDirection.Descending)
                                        .ToString();

            // assert
            Assert.AreEqual(expectedQuery, actualQuery);
        }
    }
}