using System;
using System.Collections.Generic;
using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Fixtures
{
    public class AuthorFixture : FixtureBase
    {
        public const string FirstAuthorId = "A6C5CAF9-1C81-4060-BA38-9B2ADB86D785";
        public const string SecondAuthorId = "4262018D-5047-448B-ABF2-8A3981AC4949";
        public const string ThirdAuthorId = "05706561-CC1A-436E-854A-C03D5151B922";
        public const string ForthAuthorId = "6A0230C5-79A3-42C4-887C-E93BF37036BD";
        public const string AuthorToUpdateId = "07FC0BE1-F62B-4FF6-8712-A179E864C631";
        public const string AuthorToDeleteId = "C8C18D70-0672-4F47-B1ED-AFAFD6FA5AAC";

        public static AuthorModel FirstAuthor = new AuthorModel
            {
                Id = new Guid(FirstAuthorId),
                FirstName = "firstname",
                LastName = "lastname",
                Email = "firstemail",
                Courses = new List<CourseModel> {CourseFixture.FirstCourse}
            };

        public static AuthorModel SecondAuthor = new AuthorModel
            {
                Id = new Guid(SecondAuthorId),
                FirstName = "secondname",
                LastName = "lastname",
                Email = "secondemail",
                Courses = new List<CourseModel> {CourseFixture.SecondCourse}
            };

        public static AuthorModel ThirdAuthor = new AuthorModel
            {
                Id = new Guid(ThirdAuthorId),
                FirstName = "thirdname",
                LastName = "lastname",
                Email = "thirdemail",
                Courses = new List<CourseModel> {CourseFixture.ThirdCourse}
            };

        public static AuthorModel ForthAuthor = new AuthorModel
            {
                Id = new Guid(ForthAuthorId),
                FirstName = "forthname",
                LastName = "lastname",
                Email = "forthemail",
                Courses = new List<CourseModel> {CourseFixture.ForthCourse}
            };

        public static AuthorModel AuthorToUpdate = new AuthorModel
            {
                Id = new Guid(AuthorToUpdateId),
                FirstName = "updatable User",
                LastName = "updatable user last",
                Email = "updatable@user.email",
                Courses = new List<CourseModel> {CourseFixture.CourseToUpdate}
            };

        public static AuthorModel AuthorToDelete = new AuthorModel
            {
                Id = new Guid(AuthorToDeleteId),
                FirstName = "deletable User",
                LastName = "deletable user last",
                Email = "deletable@user.email",
                Courses = new List<CourseModel> {CourseFixture.CourseToDelete}
            };

        public static void SetupFixtureDataInDatabase(IDbProvider dbProvider)
        {
            dbProvider.Create(UpdateBaseFields(FirstAuthor));
            dbProvider.Create(UpdateBaseFields(SecondAuthor));
            dbProvider.Create(UpdateBaseFields(ThirdAuthor));
            dbProvider.Create(UpdateBaseFields(ForthAuthor));
            dbProvider.Create(UpdateBaseFields(AuthorToUpdate));
            dbProvider.Create(UpdateBaseFields(AuthorToDelete));
        }
    }
}