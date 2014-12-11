using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using FutureState.AppCore.Data.Tests.Helpers.Fixtures;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Integration
{
    [TestFixture]
    public class SimpleCrudTests : IntegrationTestBase
    {
        [Test, TestCaseSource("DbProviders")]
        public void Should_Do_Crud_On_Simple_Model_Object(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Create
            var expectedAuthor = AuthorFixture.GetFirstAuthor();
            db.Create(expectedAuthor);

            // Assert Create
            var actualAuthor = db.Query<AuthorModel>().Where(a => a.Email == expectedAuthor.Email).Select().SingleOrDefault();
            actualAuthor.Should().NotBeNull();
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor);

            // Update
            expectedAuthor.FirstName = "Bob";
            expectedAuthor.LastName = "Jones";
            db.Update(expectedAuthor);

            // Assert Update
            actualAuthor = db.Query<AuthorModel>().Where(a => a.Email == expectedAuthor.Email).Select().SingleOrDefault();
            actualAuthor.Should().NotBeNull();
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor);

            // Delete
            db.Delete<AuthorModel>( x => x.Id == actualAuthor.Id);

            // Assert Delete
            actualAuthor = db.Query<AuthorModel>().Where(a => a.Email == expectedAuthor.Email).Select().SingleOrDefault();
            actualAuthor.Should().BeNull();
        }

   }
}