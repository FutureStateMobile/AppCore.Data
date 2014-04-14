using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FutureState.AppCore.Data;
using FutureState.AppCore.Tests.Helpers.Migrations;
using FutureState.AppCore.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Tests.Integration
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
            db.Delete<StudentModel>( u => u.Email == expectedUser.Email );

            var actualUser = db.Query<StudentModel>().Where( u => u.Email == expectedUser.Email ).Select().ToList();
            
            // Assert
            Assert.IsEmpty(actualUser);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldDoCrud(IDbProvider db)
        {
            // don't run against SQLite because it's not seeded.
            var provider = db.GetType().ToString();
            if ( provider == "FutureState.AppCore.Data.Sqlite.Windows.DbProvider" ) return;

            Trace.WriteLine( TraceObjectGraphInfo( db ) );

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
            Assert.IsNotNull( student );
            Assert.IsNotNull( student.Id );
            Assert.AreNotEqual(Guid.Empty, student.Id);

            // Execute Find IEnumerable
            var actualUsers1 = db.Query<StudentModel>().Where(x => x.FirstName.Contains("jil")).Select(); // this returns an IEnumerable

            // Assert Find IEnumerable
            Assert.True(actualUsers1.Any());

            // Execute Find List
            var actualUsers2 = db.Query<StudentModel>().Where(x => x.FirstName.Contains("jil") && x.LastName == "").Select().ToList();
                // ToList converts IEnumerable to a list

            // Assert Find List
            Assert.True(actualUsers2.Count > 0);

            // Execute Find List
            var stamp = DateTime.UtcNow.AddDays(-10);
            var actualUsers3 = db.Query<StudentModel>().Where( x => x.UpdatedDate >= stamp ).Select().ToList();

            // Assert Find List
            Assert.True(actualUsers3.Count > 0);

            // Execute Read
            var actualUser = db.Query<StudentModel>().Where( x => x.Id == student.Id ).Select().FirstOrDefault();

            // Assert Read
            Assert.IsNotNull(actualUser);
            Assert.AreEqual( expectedUser.FirstName, actualUser.FirstName );
            Assert.AreEqual( expectedUser.LastName, actualUser.LastName );
            Assert.AreEqual( expectedUser.Email, actualUser.Email );

            // Execute Update
            actualUser.FirstName = "NewName";
            db.Update( actualUser );
            actualUser = db.Query<StudentModel>().Where( x => x.Id == student.Id ).Select().FirstOrDefault();

            //// Assert Update
            Assert.IsNotNull(actualUser);
            Assert.AreEqual( "NewName", actualUser.FirstName );
            Assert.AreEqual( expectedUser.LastName, actualUser.LastName );
            Assert.AreEqual( expectedUser.Email, actualUser.Email );

            // Execute Delete
            db.Query<StudentModel>().Where( u => u.Id == student.Id ).Delete();
            actualUser = db.Query<StudentModel>().Where( x => x.Id == student.Id ).Select().FirstOrDefault();

            // Assert Delete
            Assert.IsNull( actualUser );
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldFindUserWithDateComparer(IDbProvider db)
        {
            Trace.WriteLine( TraceObjectGraphInfo( db ) );

            // Setup
            var stamp = DateTime.UtcNow.AddDays(-10);
            // Execute
            var actualUsers = db.Query<StudentModel>().Where(u => u.CreatedDate > stamp).Select().ToList();

            // Assert
            Assert.Greater(actualUsers.Count, 0);
        }

        [Test, TestCaseSource("DbProviders")]
        public void ShouldFindUsersWithLikeAndEqualsComparers ( IDbProvider db )
        {
            // don't run against SQLite becase it's not seeded.
            if ( db.GetType().ToString() == "FutureState.AppCore.Data.Sqlite.Windows.DbProvider" ) return;

            Trace.WriteLine( TraceObjectGraphInfo( db ) );

            // Setup
            var expectedUsers = new List<StudentModel> {new StudentModel {FirstName = "Jill"}};

            // Execute
            var actualUsers = db.Query<StudentModel>().Where(u => u.FirstName.Contains("il") && u.LastName == "").Select().ToList();

            // Assert
            Assert.AreEqual(expectedUsers[0].FirstName, actualUsers[0].FirstName);
        }

        [Test, TestCaseSource( "DbProviders" )]
        public void ShouldFindUsersUsingSameFieldTwice ( IDbProvider db )
        {
            Trace.WriteLine( TraceObjectGraphInfo( db ) );

            // Setup
            var expectedUsers = new List<StudentModel> { 
                new StudentModel { FirstName = "Bob" },
                new StudentModel { FirstName = "Jill" } 
            };

            // Execute
            var actualUsers = db.Query<StudentModel>()
                                .Where( u => u.FirstName.Contains( "jil" ) || u.FirstName == "Bob" )
                                .Select()
                                .ToList();

            // Assert
            Assert.AreEqual( expectedUsers[0].FirstName, actualUsers[0].FirstName );
            Assert.AreEqual( expectedUsers[1].FirstName, actualUsers[1].FirstName );
        }

        [Ignore, Test, TestCaseSource( "DbProviders" )]
        public void ShouldJoinToAnotherOneToManyTableAndBuildWhereClauseAndOrderByClause ( IDbProvider db )
        {
            // TODO: Implement This
            Trace.WriteLine( TraceObjectGraphInfo( db ) );

            // Setup
            var expectedUsers = new List<StudentModel> { 
                new StudentModel { FirstName = "User1" }
            };

            // Execute
            var actualUsers = db.Query<StudentModel>()
                                .Join<BookModel>(JoinType.Left, (u, b) => u.Id == b.StudentId )
                                .Where( (u, b) => b.Name == "Book1Name")
                                .OrderBy( (u, b) => u.FirstName, OrderDirection.Descending )
                                .Select()
                                .ToList();

            // Assert
            Assert.AreEqual( expectedUsers[0].FirstName, actualUsers[0].FirstName );
        }

        [Ignore, Test, TestCaseSource( "DbProviders" )]
        public void ShouldJoinToAnotherManyToManyTableAndBuildWhereClauseAndOrderByClause ( IDbProvider db )
        {
            // TODO: Implement This
            Trace.WriteLine( TraceObjectGraphInfo( db ) );

            // Setup
            var expectedUsers = new List<StudentModel> { 
                new StudentModel { FirstName = "Bob" }
            };

            // Execute
            var actualUsers = db.Query<StudentModel>()
                                .Join<CourseModel>(JoinType.ManyToMany)
                                .Where( ( s, c ) => c.Name == "Math 101" )
                                .OrderBy( (s, c) => s.FirstName, OrderDirection.Descending )
                                .Select()
                                .ToList();

            // Assert
            Assert.AreEqual( expectedUsers[0].FirstName, actualUsers[0].FirstName );
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

        [Test, TestCaseSource("DbProviders")]
        public void ShouldVisitExpressionUsingAnInnerJoin(IDbProvider db)
        {
            // don't run against SQLite becase it's not seeded.
            if ( db.GetType().ToString() == "FutureState.AppCore.Data.Sqlite.Windows.DbProvider" ) return;

            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var roleId = Migration001.MathCourseId;

            // Execute Query
            var actualUsers = db.Query<StudentModel, CourseModel>(roleId).ToList();

            // Assert
            Assert.IsNotEmpty(actualUsers);
        }
    }
}