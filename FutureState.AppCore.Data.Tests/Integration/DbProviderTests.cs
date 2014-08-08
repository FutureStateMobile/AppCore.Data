using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FutureState.AppCore.Data.Tests.Helpers.Fixtures;
using FutureState.AppCore.Data.Tests.Helpers.Migrations;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Integration
{
    [TestFixture]
    public class DbProviderTests : DbProviderTestBase
    {
        [Test, TestCaseSource("DbProviders")]
        public void ShouldDeleteAUserByEmailAddress(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var expectedUser = new StudentModel
            {
                Id = new Guid("57F98915-DBDF-41C7-9D24-F4BB1C0D9D0C"),
                FirstName = "Joe",
                LastName = "Blow",
                Email = "JoeBlow@microsoft.com",
            };
            db.Create(expectedUser);

            // Execute
            db.Delete<StudentModel>(u => u.Email == expectedUser.Email);

            var actualUser = db.Query<StudentModel>().Where(u => u.Email == expectedUser.Email).Select().ToList();

            // Assert
            Assert.IsEmpty(actualUser);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldDoCrud(IDbProvider db)
        {
            // don't run against SQLite because it's not seeded.
            var provider = db.GetType().ToString();
            if (provider == "FutureState.AppCore.Data.Sqlite.Windows.DbProvider") return;

            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var expectedUser = new StudentModel
            {
                Id = new Guid("381BC8C2-AF5D-40E1-81DD-620B4DCCEDBB"),
                FirstName = "SQL",
                LastName = "Admin",
                Email = "sa@microsoft.com",
            };

            // Execute Create
            db.Create(expectedUser);
            var student = db.Query<StudentModel>().Where(u => u.Id == expectedUser.Id).Select().FirstOrDefault();

            // Assert Create
            Assert.IsNotNull(student);
            Assert.IsNotNull(student.Id);
            Assert.AreNotEqual(Guid.Empty, student.Id);

            // Execute Find IEnumerable
            var actualUsers1 = db.Query<StudentModel>().Where(x => x.FirstName.Contains("jil")).Select();
            // this returns an IEnumerable

            // Assert Find IEnumerable
            Assert.True(actualUsers1.Any());

            // Execute Find List
            var actualUsers2 =
                db.Query<StudentModel>().Where(x => x.FirstName.Contains("jil") && x.LastName == "").Select().ToList();
            // ToList converts IEnumerable to a list

            // Assert Find List
            Assert.True(actualUsers2.Count > 0);

            // Execute Find List
            var stamp = DateTime.UtcNow.AddDays(-10);
            var actualUsers3 = db.Query<StudentModel>().Where(x => x.UpdatedDate >= stamp).Select().ToList();

            // Assert Find List
            Assert.True(actualUsers3.Count > 0);

            // Execute Read
            var actualUser = db.Query<StudentModel>().Where(x => x.Id == student.Id).Select().FirstOrDefault();

            // Assert Read
            Assert.IsNotNull(actualUser);
            Assert.AreEqual(expectedUser.FirstName, actualUser.FirstName);
            Assert.AreEqual(expectedUser.LastName, actualUser.LastName);
            Assert.AreEqual(expectedUser.Email, actualUser.Email);

            // Execute Update
            actualUser.FirstName = "NewName";
            db.Update(actualUser);
            actualUser = db.Query<StudentModel>().Where(x => x.Id == student.Id).Select().FirstOrDefault();

            //// Assert Update
            Assert.IsNotNull(actualUser);
            Assert.AreEqual("NewName", actualUser.FirstName);
            Assert.AreEqual(expectedUser.LastName, actualUser.LastName);
            Assert.AreEqual(expectedUser.Email, actualUser.Email);

            // Execute Delete
            db.Query<StudentModel>().Where(u => u.Id == student.Id).Delete();
            actualUser = db.Query<StudentModel>().Where(x => x.Id == student.Id).Select().FirstOrDefault();

            // Assert Delete
            Assert.IsNull(actualUser);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldDoCrudWithGeese(IDbProvider db)
        {
            // don't run against SQLite because it's not seeded.
            var provider = db.GetType().ToString();
            if (provider == "FutureState.AppCore.Data.Sqlite.Windows.DbProvider") return;

            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Execute Create
            var firstGoose = new GooseModel {Id = new Guid("43F4C249-E24C-41A7-9DED-73E3AE2C17BE"), Name = "My New Goose"};
            db.Create(firstGoose);
            var goose = db.Query<GooseModel>().Where(u => u.Id == firstGoose.Id).Select().FirstOrDefault();

            // Assert Create
            Assert.IsNotNull(goose);
            Assert.IsNotNull(goose.Id);
            Assert.AreEqual(firstGoose.Name, goose.Name);
            Assert.AreNotEqual(Guid.Empty, goose.Id);

            // Execute Find IEnumerable
            var actualGeese = db.Query<GooseModel>().Where(x => x.Name.Contains("irst")).Select();
            // this returns an IEnumerable

            // Assert Find IEnumerable
            Assert.True(actualGeese.Any());

            // Execute Find List
            var actualGeese2 = db.Query<GooseModel>().Where(x => x.Name.Contains("Goose")).Select().ToList();

            // Assert Find List
            Assert.True(actualGeese2.Count == 4);

            // Execute Update
            var gooseToUpdate = GooseFixture.GooseToUpdate;
            gooseToUpdate.Name = "Canada Goose";
            db.Update(gooseToUpdate);
            var actualUpdatedGoose = db.Query<GooseModel>().Where(x => x.Id == gooseToUpdate.Id).Select().FirstOrDefault();

            //// Assert Update
            Assert.IsNotNull(actualUpdatedGoose);
            Assert.AreEqual("Canada Goose", actualUpdatedGoose.Name);

            // Execute Delete
            var gooseToDelete = GooseFixture.GooseToDelete;
            db.Query<GooseModel>().Where(u => u.Id == gooseToDelete.Id).Delete();
            var actualDeletedGoose = db.Query<GooseModel>().Where(x => x.Id == gooseToDelete.Id).Select().FirstOrDefault();

            // Assert Delete
            Assert.IsNull(actualDeletedGoose);

            db.Query<GooseModel>().Truncate();

            var emptyResults = db.Query<GooseModel>().Select();
            Assert.IsEmpty(emptyResults);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldFindUserWithDateComparer(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var stamp = DateTime.UtcNow.AddDays(-10);
            // Execute
            var actualUsers = db.Query<StudentModel>().Where(u => u.CreatedDate > stamp).Select().ToList();

            // Assert
            Assert.Greater(actualUsers.Count, 0);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldFindUsersUsingSameFieldTwice(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var expectedUsers = new List<StudentModel>
            {
                new StudentModel {FirstName = "Jill"},
                new StudentModel {FirstName = "Bob"}
            };

            // Execute
            var actualUsers = db.Query<StudentModel>()
                .Where(u => u.FirstName.Contains("jil") || u.FirstName == "Bob")
                .Select()
                .ToList();

            // Assert
            Assert.AreEqual(expectedUsers[0].FirstName, actualUsers[0].FirstName);
            Assert.AreEqual(expectedUsers[1].FirstName, actualUsers[1].FirstName);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldFindUsersWithLikeAndEqualsComparers(IDbProvider db)
        {
            // don't run against SQLite becase it's not seeded.
            if (db.GetType().ToString() == "FutureState.AppCore.Data.Sqlite.Windows.DbProvider") return;

            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var expectedUsers = new List<StudentModel> {new StudentModel {FirstName = "Jill"}};

            // Execute
            var actualUsers =
                db.Query<StudentModel>().Where(u => u.FirstName.Contains("il") && u.LastName == "").Select().ToList();

            // Assert
            Assert.AreEqual(expectedUsers[0].FirstName, actualUsers[0].FirstName);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldGetAllUsers(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Execute Query
            var actualUsers = db.Query<StudentModel>().Select().ToList();

            // Assert
            Assert.IsNotEmpty(actualUsers);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldGetUserById(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            var expectedUser = new StudentModel
            {
                Id = new Guid("5A7685A2-3EEC-442D-902F-D2022F28DD33"),
                FirstName = "test",
                LastName = "test",
                Email = "test@microsoft.com",
            };

            // Execute Create
            db.Create(expectedUser);

            // Execute Query
            var actualUser = db.Query<StudentModel>().Where(u => u.Id == expectedUser.Id).Select().FirstOrDefault();

            // Assert
            Assert.IsNotNull(actualUser);
            Assert.AreEqual(actualUser.Id, expectedUser.Id);
        }

        [Ignore, Test, TestCaseSource("DbProviders")]
        public void ShouldJoinToAnotherManyToManyTableAndBuildWhereClauseAndOrderByClause(IDbProvider db)
        {
            // TODO: Implement This, data-dependent
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var expectedUsers = new List<StudentModel>
            {
                new StudentModel {FirstName = "Bob"}
            };

            // Execute
            var actualUsers = db.Query<StudentModel>()
                .Join<CourseModel>(JoinType.ManyToMany)
                .Where((s, c) => c.Name == "Math 101")
                .OrderBy((s, c) => s.FirstName, OrderDirection.Descending)
                .Select()
                .ToList();

            // Assert
            Assert.AreEqual(expectedUsers[0].FirstName, actualUsers[0].FirstName);
        }

        [Ignore, Test, TestCaseSource("DbProviders")]
        public void ShouldJoinToAnotherOneToManyTableAndBuildWhereClauseAndOrderByClause(IDbProvider db)
        {
            // TODO: Implement This
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var expectedUsers = new List<StudentModel>
            {
                new StudentModel {FirstName = "User1"}
            };

            // Execute
            var actualUsers = db.Query<StudentModel>()
                .Join<BookModel>(JoinType.Left, (u, b) => u.Id == b.StudentId)
                .Where((u, b) => b.Name == "Book1Name")
                .OrderBy((u, b) => u.FirstName, OrderDirection.Descending)
                .Select()
                .ToList();

            // Assert
            Assert.AreEqual(expectedUsers[0].FirstName, actualUsers[0].FirstName);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldNotRunMigrations(IDbProvider db)
        {
            // Setup
            var migrationRunner = new MigrationRunner(db);

            // Execute / Assert
            Assert.DoesNotThrow(() => migrationRunner.RunAll(SystemRole.Server, new List<IMigration> {new Migration001(), new Migration002()}));
            Assert.DoesNotThrow(() => migrationRunner.RunAll(SystemRole.Client, new List<IMigration> {new Migration001(), new Migration002()}));
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldVisitExpressionUsingAnInnerJoin(IDbProvider db)
        {
            // don't run against SQLite becase it's not seeded.
            if (db.GetType().ToString() == "FutureState.AppCore.Data.Sqlite.Windows.DbProvider") return;

            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var roleId = Migration001.MathCourseId;

            // Execute Query
            var actualUsers = db.Query<StudentModel, CourseModel>(roleId).ToList();

            // Assert
            Assert.IsNotEmpty(actualUsers);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldAddToCollectionWithoutUniqueConstraintFailure(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // setup
            var studentToUpdate = StudentFixture.StudentToUpdate;
            studentToUpdate.Courses.Add(CourseFixture.ThirdCourse);

            // execute
            db.Update(studentToUpdate);
            var studentFromDb = db.Query<StudentModel>().Where(s => s.Id == studentToUpdate.Id).Select().FirstOrDefault();

            studentFromDb.Courses = db.Query<CourseModel, StudentModel>(studentFromDb.Id)
                .Where(x => x.IsDeleted == false)
                .ToList();

            // assert
            Assert.IsNotNull(studentFromDb);
            Assert.That(studentFromDb.Courses.Any(p => p.Id == CourseFixture.ThirdCourse.Id));
        }
    }
}