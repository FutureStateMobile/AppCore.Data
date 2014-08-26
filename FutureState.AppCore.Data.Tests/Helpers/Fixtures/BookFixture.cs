using System;
using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Fixtures
{
    public class BookFixture : FixtureBase
    {
        public const string FirstBookId = "6DB6BA38-9B01-4A64-A227-CC69A51BF57B";
        public const string BookToUpdateId = "486652C0-7E6E-43D3-A9F3-950B66BFFCC6";
        public const string BookToDeleteId = "3C1F3E58-8CFD-4FB9-A5D5-FCCDFFFDB73B";

        public static BookModel FirstBook = new BookModel
            {
                Id = new Guid(FirstBookId),
                StudentId = new Guid(StudentFixture.FirstStudentId),
                Name = "FirstBookTitle",
                BookNumber = 1
            };

        public static BookModel BookToUpdate = new BookModel
            {
                Id = new Guid(BookToUpdateId),
                StudentId = new Guid(StudentFixture.StudentToUpdateId),
                Name = "BookTitleToUpdate",
                BookNumber = 2
            };

        public static BookModel BookToDelete = new BookModel
            {
                Id = new Guid(BookToDeleteId),
                StudentId = new Guid(StudentFixture.StudentToDeleteId),
                Name = "BookTitleToDelete",
                BookNumber = 3
            };

        public static void SetupFixtureDataInDatabase(IDbProvider dbProvider)
        {
            dbProvider.Create(UpdateBaseFields(FirstBook));
            dbProvider.Create(UpdateBaseFields(BookToUpdate));
            dbProvider.Create(UpdateBaseFields(BookToDelete));
        }
    }
}