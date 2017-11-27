﻿using System.Diagnostics;
using FluentAssertions;
using FutureState.AppCore.Data.Tests.Helpers.Fixtures;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Integration
{
    [TestFixture]
    public class ManyToOneTests : IntegrationTestBase
    {
        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Create_Records_With_ManyToOne_Relationships(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetFirstPublisher();
            var firstBook = BookFixture.GetFirstBook(publisher);
            var secondBook = BookFixture.GetSecondBook(publisher);

            db.Create(publisher);
            db.Create(firstBook);
            db.Create(secondBook);

            // Execute
            var query = db.Query<BookModel>()
                                .Join<PublisherModel>().On((b, p) => b.Publisher.Id == p.Id)
                                .Where((b, p) => p.Id == publisher.Id);
            var actualBooks = query.ToList();
            
            // Assert
            actualBooks.Should().HaveCount(2);
            actualBooks[0].Publisher.Id.Should().Be(publisher.Id);
            //actualBooks[0].Publisher.Name.Should().Be(publisher.Name);
            actualBooks[1].Publisher.Id.Should().Be(publisher.Id);
            //actualBooks[1].Publisher.Name.Should().Be(publisher.Name);
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Create_Records_With_OneToMany_Relationships(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetSecondPublisher();
            db.Create(publisher);

            var thirdBook = BookFixture.GetThirdBook(publisher);
            var fourthBook = BookFixture.GetFourthBook(publisher);
            db.Create(thirdBook);
            db.Create(fourthBook);

            // Execute
            var query = db.Query<PublisherModel>()
                .Join<BookModel>().On((p, b) => b.Publisher.Id == p.Id)
                .Where((p, b) => b.Name == fourthBook.Name);
            var publishers = query.ToList();

            // Assert
            publishers.Should().HaveCount(1);
            publishers[0].ShouldBeEquivalentTo(publisher);
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Query_ManyToOne_Records_With_Needing_To_Build_Join(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetThirdPublisher();
            db.Create(publisher);

            var thirdBook = BookFixture.GetThirdBook(publisher);
            var fourthBook = BookFixture.GetFourthBook(publisher);

            // Execute
            db.Create(thirdBook);
            db.Create(fourthBook);

            // Assert
            var query = db.Query<BookModel>()
                               .Where(b => b.Publisher.Id == publisher.Id);
            var actualBooks = query.ToList();

            actualBooks.Should().HaveCount(2);
            actualBooks[0].Publisher.Id.Should().Be(publisher.Id);
            actualBooks[1].Publisher.Id.Should().Be(publisher.Id);
        }
    }
}
