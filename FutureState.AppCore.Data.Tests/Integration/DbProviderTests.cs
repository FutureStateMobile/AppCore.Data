using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
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
            Assert.That(actualUsers.Any(s => s.FirstName == expectedUsers[0].FirstName));
            Assert.That(actualUsers.Any(s => s.FirstName == expectedUsers[1].FirstName));
            Assert.That(actualUsers.Count(s => s.FirstName == expectedUsers[0].FirstName), Is.EqualTo(1));
            Assert.That(actualUsers.Count(s => s.FirstName == expectedUsers[1].FirstName), Is.EqualTo(1));
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
            Assert.AreEqual( expectedUser.Id, actualUser.Id );
        }

        [Test, TestCaseSource( "DbProviders" )]
        public void ShouldSelectMaxCreatedDate ( IDbProvider db )
        {
            Trace.WriteLine( TraceObjectGraphInfo( db ) );
            //const string expectedSelect = "SELECT MAX(CreatedDate) FROM Students";

            // Execute Query
            var actualSelect = db.Scalar<StudentModel, DateTime>(s => s.CreatedDate).Max();

            // Assert
            Assert.IsNotNull(actualSelect);
            //Assert.AreEqual( expectedSelect, actualSelect );
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldReturnMinDateTimeFromEmptyTable(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            //setup
            var migration = new DbMigration(db.Dialect);
            var database = new Database(db.DatabaseName, db.Dialect);
            var fakeTable = database.AddTable("Fakes");
            fakeTable.AddColumn("TimeStamp", typeof (DateTime)).Nullable();
            db.ExecuteNonQuery(migration.GenerateDDL(database));

            // Execute Query
            var actualSelect = db.Scalar<FakeModel, DateTime>(s => s.TimeStamp).Max();

            // Assert
            Assert.IsNotNull(actualSelect);
            //Assert.AreEqual( expectedSelect, actualSelect );
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldJoinToAnotherManyToManyTableAndBuildWhereClauseAndOrderByClause(IDbProvider db)
        {
            // TODO: Implement This, data-dependent
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var expectedStudents = StudentFixture.FirstStudent;
            var expectedCourse = CourseFixture.FirstCourse;

            // Execute
            var actualStudents = db.Query<StudentModel>()
                .ManyJoin<CourseModel>()
                .Where( ( s, c ) => c.Name == expectedCourse.Name )
                .OrderBy((s, c) => s.FirstName, OrderDirection.Descending)
                .Select()
                .ToList();

            // Assert
            Assert.AreEqual(1, actualStudents.Count());
            Assert.AreEqual(expectedStudents.Id, actualStudents[0].Id);
        }

        [Ignore, Test, TestCaseSource("DbProviders")]
        public void ShouldJoinToAnotherOneToManyTableAndBuildWhereClauseAndOrderByClause(IDbProvider db)
        {
//            // TODO: Implement This
//            Trace.WriteLine(TraceObjectGraphInfo(db));
//
//            // Setup
//            var expectedStudents = new List<StudentModel>
//            {
//                StudentFixture.FirstStudent
//            };
//
//            // Execute
//            var actualStudents = db.Query<StudentModel>()
//                .InnerJoin<BookModel>(JoinType.Left, (u, b) => u.Id == b.StudentId)
//                .Where((u, b) => b.Name == "Book1Name")
//                .OrderBy((u, b) => u.FirstName, OrderDirection.Descending)
//                .Select()
//                .ToList();
//
//            // Assert
//            Assert.AreEqual( expectedStudents[0].FirstName, actualStudents[0].FirstName );
        }

        [Test, TestCaseSource( "DbProviders" )]
        public void ShouldJoinToAnotherOneToManyTableAndBuildDefaultWhereClauseAndOrderByClause ( IDbProvider db )
        {
            // TODO: Implement This
            Trace.WriteLine( TraceObjectGraphInfo( db ) );

            // Setup
            var expectedStudents = new List<StudentModel>
            {
                StudentFixture.FirstStudent
            };

            // Execute
            var actualStudents = db.Query<StudentModel>()
                .LeftJoin<BookModel>()
                .Where( ( student, book ) => book.Name == "FirstBookTitle" )
                .OrderBy( ( student, book ) => student.FirstName, OrderDirection.Descending )
                .Select()
                .ToList();

            // Assert
            Assert.AreEqual(1, actualStudents.Count());
            Assert.AreEqual( expectedStudents[0].Id, actualStudents[0].Id );
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
        public void ShouldJoinManyToManyTablesTogether(IDbProvider db)
        {
            // don't run against SQLite becase it's not seeded.
            if (db.GetType().ToString() == "FutureState.AppCore.Data.Sqlite.Windows.DbProvider") return;

            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var mathCourseId = Migration001.MathCourseId;

            // Execute Query
            var actualUsers = db.Query<StudentModel>()
                .ManyJoin<CourseModel>()
                .Where( ( s, b ) => b.Id == mathCourseId )
                .Select()
                .ToList();

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
            db.Update( studentToUpdate );

            var studentCourses = db.Query<CourseModel>()
                .ManyJoin<StudentModel>()
                .Where( ( c, s ) => c.IsDeleted == false && s.Id == studentToUpdate.Id )
                .Select()
                .ToList();

            // assert
            Assert.That( studentCourses.Count(), Is.EqualTo( 2 ) );
            Assert.That( studentCourses.Any( p => p.Id == CourseFixture.ThirdCourse.Id ) );
        }

        [Test, TestCaseSource( "DbProviders" )]
        public void ShouldRemoveFromCollection ( IDbProvider db )
        {
            Trace.WriteLine( TraceObjectGraphInfo( db ) );

            // setup
            var studentToDelete = StudentFixture.StudentToDelete;
            studentToDelete.Courses.Remove(CourseFixture.CourseToDelete);

            // execute
            db.Update( studentToDelete );

            var studentCourses = db.Query<CourseModel>()
                .ManyJoin<StudentModel>()
                .Where( ( c, s ) => c.IsDeleted == false && s.Id == studentToDelete.Id )
                .Select()
                .ToList();

            // assert
            Assert.That( studentCourses.Count(), Is.EqualTo( 0 ) );
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldSayTheHighestbookNumberIsThree(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));
            const int expectedNumber = 3;

            // Execute Query
            var actualNumber = db.Scalar<BookModel, int>(s => s.BookNumber).Max();

            // Assert
            Assert.That(actualNumber, Is.EqualTo(expectedNumber));
        }
    }
}