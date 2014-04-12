using System;
using System.Collections.Generic;
using System.Globalization;
using FutureState.AppCore.Data;
using FutureState.AppCore.Tests.Helpers.Models;
using Moq;
using NUnit.Framework;

namespace FutureState.AppCore.Tests.Unit
{
    [ TestFixture ]
    public class AutoMapperTests
    {
        [ Test ]
        public void ShouldBuildDbParametersForGivenModel()
        {
            // Setup
            var expectedDate = DateTime.UtcNow;
            var expectedUser = new StudentModel
                {
                    Id = Guid.NewGuid(),
                    FirstName = "firstname",
                    LastName = "lastname",
                    Email = "email@gmail.com",
                    CreatedDate = expectedDate,
                    UpdatedDate = expectedDate,
                    IsDeleted = true
                };

            var mapper = new AutoMapper<StudentModel> ();

            // Execute
            var actual = mapper.BuildDbParametersFrom( expectedUser );

            // Assert
            Assert.AreEqual( expectedUser.Id, ( Guid ) actual["Id"] );
            Assert.AreEqual( expectedUser.FirstName, actual["FirstName"] as string );
            Assert.AreEqual( expectedUser.LastName, actual["LastName"] as string );
            Assert.AreEqual( expectedUser.Email, actual["Email"] as string );
            Assert.AreEqual( expectedUser.CreatedDate.ToString( CultureInfo.InvariantCulture ),
                             expectedDate.ToString( CultureInfo.InvariantCulture ) );
            Assert.AreEqual( expectedUser.UpdatedDate.ToString( CultureInfo.InvariantCulture ),
                             expectedDate.ToString( CultureInfo.InvariantCulture ) );
            Assert.AreEqual( expectedUser.IsDeleted, ( bool ) actual["IsDeleted"] );
        }

        [ Test ]
        public void ShouldBuildListOfModelObjectsFromReader()
        {
            // Setup
            var expectedDate = DateTime.UtcNow;

            var dataReader = new Mock<IDbReader> ();
            dataReader.SetupSequence( reader => reader.Read () ).Returns( true ).Returns( true ).Returns( true ).Returns( false );
            dataReader.SetupSequence( dr => dr["Id"] )
                      .Returns( new Guid( "4C565CBE-4003-4624-B399-A3E40096A7D0" ) )
                      .Returns( null )
                      .Returns( new Guid( "94664F60-1F40-4C98-8870-C662B3B96605" ) );
            dataReader.SetupSequence( dr => dr["FirstName"] ).Returns( "firstname1" ).Returns( "firstname2" ).Returns( "firstname2" );
            dataReader.SetupSequence( dr => dr["LastName"] ).Returns( "lastname1" ).Returns( "lastname2" ).Returns( "lastname3" );
            dataReader.SetupSequence( dr => dr["Email"] ).Returns( "email1" ).Returns( "email2" ).Returns( "email2" );
            dataReader.SetupSequence( dr => dr["CreatedDate"] ).Returns( expectedDate ).Returns( null ).Returns( expectedDate );
            dataReader.SetupSequence( dr => dr["UpdatedDate"] ).Returns( expectedDate ).Returns( expectedDate ).Returns( null );
            dataReader.SetupSequence( dr => dr["IsDeleted"] ).Returns( true ).Returns( null ).Returns( false );

            var mapper = new AutoMapper<StudentModel> ();

            // Execute
            var actualUser = mapper.BuildListFrom( dataReader.Object );

            // Assert
            Assert.AreEqual( actualUser[0].Id.ToString ().ToUpper (), "4C565CBE-4003-4624-B399-A3E40096A7D0" );
            Assert.AreEqual( actualUser[0].FirstName, "firstname1" );
            Assert.AreEqual( actualUser[0].LastName, "lastname1" );
            Assert.AreEqual( actualUser[0].Email, "email1" );
            Assert.AreEqual( actualUser[0].CreatedDate.ToString( CultureInfo.InvariantCulture ),
                             expectedDate.ToString( CultureInfo.InvariantCulture ) );
            Assert.AreEqual( actualUser[0].UpdatedDate.ToString( CultureInfo.InvariantCulture ),
                             expectedDate.ToString( CultureInfo.InvariantCulture ) );
            Assert.AreEqual( actualUser[0].IsDeleted, true );

            Assert.AreEqual( actualUser[1].Id.ToString ().ToUpper (), Guid.Empty.ToString () );
            Assert.AreEqual( actualUser[1].FirstName, "firstname2" );
            Assert.AreEqual( actualUser[1].LastName, "lastname2" );
            Assert.AreEqual( actualUser[1].Email, "email2" );
            Assert.AreEqual( actualUser[1].CreatedDate.ToString( CultureInfo.InvariantCulture ),
                             DateTime.MinValue.ToString( CultureInfo.InvariantCulture ) );
            Assert.AreEqual( actualUser[1].UpdatedDate.ToString( CultureInfo.InvariantCulture ),
                             expectedDate.ToString( CultureInfo.InvariantCulture ) );
            Assert.AreEqual( actualUser[1].IsDeleted, false );
        }

        [ Test ]
        public void ShouldBuildListOfObjectFromAnotherListOfObjects()
        {
            // Setup
            var books = new List<BookModel>
            {
                new BookModel { Name = "bob", Id = new Guid( "645301E1-EC02-47BC-A99C-330A77FC6A4E" ) },
                new BookModel { Name = "Joe", Id = new Guid( "02CEF568-4130-457C-A34C-268585AFB939" ) },
            };
            var autoMapper = new AutoMapper<CourseModel> ();

            // Execute
            var courses = autoMapper.BuildListFrom( books );

            // Assert
            Assert.AreEqual( books[0].Name, courses[0].Name );
            Assert.AreEqual( books[1].Name, courses[1].Name );
        }

        [ Test ]
        public void ShouldBuildModelObjectFromReader()
        {
            // Setup
            var expectedDate = DateTime.UtcNow;
            var dataReader = new Mock<IDbReader> ();
            dataReader.SetupSequence( reader => reader.Read () ).Returns( true ).Returns( false );
            dataReader.SetupSequence( dr => dr["Id"] ).Returns( new Guid( "4C565CBE-4003-4624-B399-A3E40096A7D0" ) );
            dataReader.SetupSequence( dr => dr["FirstName"] ).Returns( "firstname1" );
            dataReader.SetupSequence( dr => dr["LastName"] ).Returns( "lastname1" );
            dataReader.SetupSequence( dr => dr["Email"] ).Returns( "email1" );
            dataReader.SetupSequence( dr => dr["PasswordHash"] ).Returns( "passwordhash1" );
            dataReader.SetupSequence( dr => dr["CreatedDate"] ).Returns( expectedDate );
            dataReader.SetupSequence( dr => dr["UpdatedDate"] ).Returns( expectedDate );
            dataReader.SetupSequence( dr => dr["IsDeleted"] ).Returns( true );

            var mapper = new AutoMapper<StudentModel> ();

            // Execute
            var actualUser = mapper.BuildFrom( dataReader.Object );

            // Assert
            Assert.AreEqual( actualUser.Id.ToString (), "4c565cbe-4003-4624-b399-a3e40096a7d0" );
            Assert.AreEqual( actualUser.FirstName, "firstname1" );
            Assert.AreEqual( actualUser.LastName, "lastname1" );
            Assert.AreEqual( actualUser.Email, "email1" );
            Assert.AreEqual( actualUser.CreatedDate.ToString( CultureInfo.InvariantCulture ),
                             expectedDate.ToString( CultureInfo.InvariantCulture ) );
            Assert.AreEqual( actualUser.UpdatedDate.ToString( CultureInfo.InvariantCulture ),
                             expectedDate.ToString( CultureInfo.InvariantCulture ) );
            Assert.AreEqual( actualUser.IsDeleted, true );
        }

        [ Test ]
        public void ShouldBuildOneObjectFromAnotherObjectWhereNamesMatch()
        {
            // Setup
            var book = new BookModel { Name = "bob", Id = new Guid( "645301E1-EC02-47BC-A99C-330A77FC6A4E" ) };
            var autoMapper = new AutoMapper<CourseModel> ();

            // Execute
            var course = autoMapper.BuildFrom( book );

            // Assert
            Assert.AreEqual( book.Name, course.Name );
        }

        [ Test ]
        public void ShouldNotCrashIfInputIsNull()
        {
            // Setup
            BookModel book = null;
            var autoMapper = new AutoMapper<CourseModel> ();

            // Execute
            var course = autoMapper.BuildFrom( book );

            // Assert
            Assert.IsNull( course );
        }
    }
}