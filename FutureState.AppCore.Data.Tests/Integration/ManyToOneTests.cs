using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using FutureState.AppCore.Data.Tests.Helpers.Fixtures;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Integration
{
    [TestFixture]
    public class ManyToOneTests : IntegrationTestBase
    {
        [Test, TestCaseSource("DbProviders")]
        public void Should_Create_Records_With_ManyToOne_Relationships(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetFirstPublisher();
            db.Create(publisher);

            var firstBook = BookFixture.GetFirstBook(publisher);
            var secondBook = BookFixture.GetSecondBook(publisher);

            // Execute
            db.Create(firstBook);
            db.Create(secondBook);

            // Assert
            var actualBooks = db.Query<BookModel>()
                                .Join<PublisherModel>((b, p) => b.Publisher.Id == p.Id)
                                .Where((b, p) => p.Id == publisher.Id)
                                .Select().ToList();
            
            actualBooks.Should().HaveCount(2);
            actualBooks[0].Publisher.Id.Should().Be(publisher.Id);
            actualBooks[1].Publisher.Id.Should().Be(publisher.Id);
        }


        [Test, TestCaseSource("DbProviders")]
        public void Should_Create_Records_With_OneToMany_Relationships(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetSecondPublisher();
            db.Create(publisher);

            var thirdBook = BookFixture.GetThirdBook(publisher);
            var fourthBook = BookFixture.GetFourthBook(publisher);

            // Execute
            db.Create(thirdBook);
            db.Create(fourthBook);

            // Assert
            var publishers = db.Query<PublisherModel>()
                               .Join<BookModel>((p, b) => b.Publisher.Id == p.Id)
                               .Where((p, b) => b.Name == fourthBook.Name)
                               .Select().ToList();

            publishers.Should().HaveCount(1);
            publishers[0].ShouldBeEquivalentTo(publisher);
        }
    }
}
