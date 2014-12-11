using System;
using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Fixtures
{
    public class AuthorFixture : FixtureBase
    {
        public static AuthorModel GetFirstAuthor()
        {
            return UpdateBaseFields(new AuthorModel
            {
                Id = Guid.NewGuid(),
                FirstName = "firstname",
                LastName = "lastname",
                Email = "firstemail",
            });
        }

        public static AuthorModel GetSecondAuthor()
        {
            return UpdateBaseFields(new AuthorModel
            {
                Id = Guid.NewGuid(),
                FirstName = "secondname",
                LastName = "lastname",
                Email = "secondemail",
            });
        }

        public static AuthorModel GetThirdAuthor()
        {
            return UpdateBaseFields(new AuthorModel
            {
                Id = Guid.NewGuid(),
                FirstName = "thirdname",
                LastName = "lastname",
                Email = "thirdemail",
            });
        }

        public static AuthorModel GetFourthAuthor()
        {
            return UpdateBaseFields(new AuthorModel
            {
                Id = Guid.NewGuid(),
                FirstName = "fourthname",
                LastName = "lastname",
                Email = "fourthemail",
            });
        }

        public static AuthorModel GetAuthorToUpdate()
        {
            return UpdateBaseFields(new AuthorModel
            {
                Id = Guid.NewGuid(),
                FirstName = "updatable User",
                LastName = "updatable user last",
                Email = "updatable@user.email",
            });
        }

        public static AuthorModel GetAuthorToDelete()
        {
            return UpdateBaseFields(new AuthorModel
            {
                Id = Guid.NewGuid(),
                FirstName = "deletable User",
                LastName = "deletable user last",
                Email = "deletable@user.email",
            });
        }
    }
}