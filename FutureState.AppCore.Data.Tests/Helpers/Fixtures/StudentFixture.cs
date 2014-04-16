using System;
using System.Collections.Generic;
using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Fixtures
{
    public class StudentFixture : FixtureBase
    {
        public const string FirstStudentId = "A6C5CAF9-1C81-4060-BA38-9B2ADB86D785";
        public const string SecondStudentId = "4262018D-5047-448B-ABF2-8A3981AC4949";
        public const string ThirdStudentId = "05706561-CC1A-436E-854A-C03D5151B922";
        public const string ForthStudentId = "6A0230C5-79A3-42C4-887C-E93BF37036BD";
        public const string StudentToUpdateId = "07FC0BE1-F62B-4FF6-8712-A179E864C631";
        public const string StudentToDeleteId = "C8C18D70-0672-4F47-B1ED-AFAFD6FA5AAC";

        public static StudentModel FirstStudent = new StudentModel
            {
                Id = new Guid( FirstStudentId ),
                FirstName = "firstname",
                LastName = "lastname",
                Email = "firstemail",
                Courses = new List<CourseModel>{CourseFixture.FirstCourse}
            };

        public static StudentModel SecondStudent = new StudentModel
            {
                Id = new Guid( SecondStudentId ),
                FirstName = "secondname",
                LastName = "lastname",
                Email = "secondemail",
                Courses = new List<CourseModel> { CourseFixture.SecondCourse }
            };

        public static StudentModel ThirdStudent = new StudentModel
        {
            Id = new Guid(ThirdStudentId),
            FirstName = "thirdname",
            LastName = "lastname",
            Email = "thirdemail",
            Courses = new List<CourseModel> { CourseFixture.ThirdCourse }
        };

        public static StudentModel ForthStudent = new StudentModel
        {
            Id = new Guid(ForthStudentId),
            FirstName = "forthname",
            LastName = "lastname",
            Email = "forthemail",
            Courses = new List<CourseModel> { CourseFixture.ForthCourse }
        };

        public static StudentModel StudentToUpdate = new StudentModel
        {
            Id = new Guid(StudentToUpdateId),
            FirstName = "updatable User",
            LastName = "updatable user last",
            Email = "updatable@user.email",
            Courses = new List<CourseModel> { CourseFixture.FirstCourse }
        };

        public static StudentModel StudentToDelete = new StudentModel
        {
            Id = new Guid(StudentToDeleteId),
            FirstName = "deletable User",
            LastName = "deletable user last",
            Email = "deletable@user.email",
            Courses = new List<CourseModel> { CourseFixture.FirstCourse }
        };

        public static void SetupFixtureDataInDatabase(IDbProvider dbProvider)
        {
            dbProvider.Create(UpdateBaseFields(FirstStudent));
            dbProvider.Create(UpdateBaseFields(SecondStudent));
            dbProvider.Create(UpdateBaseFields(ThirdStudent));
            dbProvider.Create(UpdateBaseFields(ForthStudent));
            dbProvider.Create(UpdateBaseFields(StudentToUpdate));
            dbProvider.Create(UpdateBaseFields(StudentToDelete));
        }
    }
}