using System;
using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Fixtures
{
    public class BookFixture : FixtureBase
    {
        private static readonly DateTime Date = new DateTime(2014, 12, 1);

        public static BookModel GetFirstBook(PublisherModel publisher = null)
        {
            return UpdateBaseFields(new BookModel
            {
                Id = Guid.NewGuid(),
                Name = "FirstBookTitle",
                ISBN = 1,
                Publisher = publisher,
                PublishDate = Date
            });
        }

        public static BookModel GetSecondBook(PublisherModel publisher = null)
        {
            return UpdateBaseFields(new BookModel
            {
                Id = Guid.NewGuid(),
                Name = "SecondBookTitle",
                ISBN = 1,
                Publisher = publisher,
                PublishDate = Date
            });
        }

        public static BookModel GetThirdBook(PublisherModel publisher = null)
        {
            return UpdateBaseFields(new BookModel
            {
                Id = Guid.NewGuid(),
                Name = "ThirdBookTitle",
                ISBN = 1,
                Publisher = publisher,
                PublishDate = Date
            });
        }

        public static BookModel GetFourthBook(PublisherModel publisher = null)
        {
            return UpdateBaseFields(new BookModel
            {
                Id = Guid.NewGuid(),
                Name = "FourthBookTitle",
                ISBN = 1,
                Publisher = publisher,
                PublishDate = Date
            });
        }

        public static BookModel GetBookToUpdate(PublisherModel publisher = null)
        {
            return UpdateBaseFields(new BookModel
            {
                Id = Guid.NewGuid(),
                Name = "BookTitleToUpdate",
                ISBN = 2,
                Publisher = publisher,
                PublishDate = Date
            });
        }


        public static BookModel GetBookToDelete(PublisherModel publisher = null)
        {
            return UpdateBaseFields(new BookModel
            {
                Id = Guid.NewGuid(),
                Name = "BookTitleToDelete",
                ISBN = 3,
                Publisher = publisher,
                PublishDate = Date
            });
        }
    }
}