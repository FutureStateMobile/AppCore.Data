using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using FutureState.AppCore.Data.Tests.Helpers.Fixtures;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Integration
{
    [TestFixture]
    public class ManyToManyTests : IntegrationTestBase
    {
        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Create_Records_With_ManyToMany_Relationships(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetFirstPublisher();
            db.Create(publisher);

            var firstBook = BookFixture.GetFirstBook(publisher);
            var secondBook = BookFixture.GetSecondBook(publisher);
            db.Create(firstBook);
            db.Create(secondBook);

            var expectedAuthor = AuthorFixture.GetFirstAuthor();
            expectedAuthor.AddBooks(firstBook, secondBook);

            // Execute
            db.Create(expectedAuthor);

            // Assert
            var actualAuthor = db.Query<AuthorModel>()
                                 .Where(a => a.Email == expectedAuthor.Email)
                                 .Select()
                                 .Single();
            
            actualAuthor.Should().NotBeNull();

            actualAuthor.AddBooks(db.Query<BookModel>()
                                    .ManyToManyJoin<AuthorModel>()
                                    .Where((b, a) => a.Id == actualAuthor.Id)
                                    .Select().ToArray());

            // Excluding publisher info because only its ID is included in the hydration.
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor, options => options.Excluding(a => a.PropertyPath.Contains("Publisher")));
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Add_Records_To_ManyToMany_Relationship(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetSecondPublisher();
            db.Create(publisher);

            var firstBook = BookFixture.GetFirstBook(publisher);
            var secondBook = BookFixture.GetSecondBook(publisher);
            db.Create(firstBook);
            db.Create(secondBook);

            var expectedAuthor = AuthorFixture.GetSecondAuthor();
            db.Create(expectedAuthor);
            
            // Execute
            expectedAuthor.AddBooks(firstBook, secondBook);
            db.Update(expectedAuthor);

            // Assert
            var actualAuthor = db.Query<AuthorModel>()
                                 .Where(a => a.Email == expectedAuthor.Email)
                                 .Select().Single();

            actualAuthor.Should().NotBeNull();

            actualAuthor.AddBooks(db.Query<BookModel>()
                                    .ManyToManyJoin<AuthorModel>()
                                    .Where((b, a) => a.Id == actualAuthor.Id)
                                    .Select().ToArray());

            // Excluding publisher info because only its ID is included in the hydration.
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor, options => options.Excluding(a => a.PropertyPath.Contains("Publisher")));
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Remove_Records_From_ManyToMany_Relationship(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetThirdPublisher();
            db.Create(publisher);

            var firstBook = BookFixture.GetFirstBook(publisher);
            var secondBook = BookFixture.GetSecondBook(publisher);
            db.Create(firstBook);
            db.Create(secondBook);

            var expectedAuthor = AuthorFixture.GetThirdAuthor();
            expectedAuthor.AddBooks(firstBook, secondBook);
            db.Create(expectedAuthor);

            // Execute
            expectedAuthor.RemoveBooks(firstBook);
            db.Update(expectedAuthor);

            // Assert
            var actualAuthor = db.Query<AuthorModel>()
                                 .Where(a => a.Email == expectedAuthor.Email)
                                 .Select().Single();

            actualAuthor.Should().NotBeNull();

            actualAuthor.AddBooks(db.Query<BookModel>()
                                    .ManyToManyJoin<AuthorModel>()
                                    .Where((b, a) => a.Id == actualAuthor.Id)
                                    .Select().ToArray());

            // Excluding publisher info because only its ID is included in the hydration.
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor, options => options.Excluding(a => a.PropertyPath.Contains("Publisher")));
        }
        
        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Create_Records_With_ManyToMany_Relationships_Using_Transactions(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetFirstPublisher();
            var firstBook = BookFixture.GetFirstBook(publisher);
            var secondBook = BookFixture.GetSecondBook(publisher);
            var expectedAuthor = AuthorFixture.GetFirstAuthor();
            expectedAuthor.AddBooks(firstBook, secondBook);

            db.RunInTransaction(trans =>
            {
                trans.Create(publisher);
                trans.Create(firstBook);
                trans.Create(secondBook);
                trans.Create(expectedAuthor);
            });

            // Assert
            var actualAuthor = db.Query<AuthorModel>()
                .Where(a => a.Email == expectedAuthor.Email)
                .Select()
                .Single();

            actualAuthor.Should().NotBeNull();

            actualAuthor.AddBooks(db.Query<BookModel>()
                .ManyToManyJoin<AuthorModel>()
                .Where((b, a) => a.Id == actualAuthor.Id)
                .Select().ToArray());

            // Excluding publisher info because only its ID is included in the hydration.
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor, options => options.Excluding(a => a.PropertyPath.Contains("Publisher")));
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Add_Records_To_ManyToMany_Relationship_Using_Transactions(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetSecondPublisher();
            var firstBook = BookFixture.GetFirstBook(publisher);
            var secondBook = BookFixture.GetSecondBook(publisher);
            var expectedAuthor = AuthorFixture.GetSecondAuthor();
            expectedAuthor.AddBooks(firstBook, secondBook);

            // execute
            db.RunInTransaction(trans =>
            {
                trans.CreateOrUpdate(publisher);
                trans.CreateOrUpdate(firstBook);
                trans.CreateOrUpdate(secondBook);
                trans.CreateOrUpdate(expectedAuthor);
                trans.CreateOrUpdate(expectedAuthor);
            });
            
            // Assert
            var actualAuthor = db.Query<AuthorModel>()
                .Where(a => a.Email == expectedAuthor.Email)
                .Select().Single();

            actualAuthor.Should().NotBeNull();

            actualAuthor.AddBooks(db.Query<BookModel>()
                .ManyToManyJoin<AuthorModel>()
                .Where((b, a) => a.Id == actualAuthor.Id)
                .Select().ToArray());

            // Excluding publisher info because only its ID is included in the hydration.
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor, options => options.Excluding(a => a.PropertyPath.Contains("Publisher")));
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Remove_Records_From_ManyToMany_Relationship_Using_Transactions(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Setup
            var publisher = PublisherFixture.GetThirdPublisher();
            var firstBook = BookFixture.GetFirstBook(publisher);
            var secondBook = BookFixture.GetSecondBook(publisher);
            var expectedAuthor = AuthorFixture.GetThirdAuthor();
            expectedAuthor.RemoveBooks(firstBook);
            expectedAuthor.AddBooks(firstBook, secondBook);

            db.RunInTransaction(trans =>
            {
                trans.Create(publisher);
                trans.Create(firstBook);
                trans.Create(secondBook);
                trans.Create(expectedAuthor);
                trans.Update(expectedAuthor);
            });

            // Assert
            var actualAuthor = db.Query<AuthorModel>()
                .Where(a => a.Email == expectedAuthor.Email)
                .Select().Single();

            actualAuthor.Should().NotBeNull();

            actualAuthor.AddBooks(db.Query<BookModel>()
                .ManyToManyJoin<AuthorModel>()
                .Where((b, a) => a.Id == actualAuthor.Id)
                .Select().ToArray());

            // Excluding publisher info because only its ID is included in the hydration.
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor, options => options.Excluding(a => a.PropertyPath.Contains("Publisher")));
        }
    }
}