using System;
using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Fixtures
{
    public class CourseFixture : FixtureBase
    {
        public const string FirstCourseId = "7BE9330E-9F03-4AAD-85E8-630E16C381B6";
        public const string SecondCourseId = "A4A879A2-DB9C-47AF-9730-EDE6488AA5B9";
        public const string ThirdCourseId = "720F3547-A9DB-41AE-9AC0-661F1FE321AA";
        public const string ForthCourseId = "D4A8838B-6F02-4EF4-A3C2-5A11601B8563";
        public const string SystemCourseId = "A47A0DEE-A265-4927-835F-54CEADFF0D7A";
        public const string CourseToUpdateId = "BE4B9ADF-EBC1-4B7C-911D-8006EC1F4F7C";
        public const string CourseToDeleteId = "13CFE951-217E-45CE-990C-33082A881951";

        public static CourseModel FirstCourse = new CourseModel
            {
                Id = new Guid(FirstCourseId),
                Name = "First",
                Description = "First course description",
            };

        public static CourseModel SecondCourse = new CourseModel
            {
                Id = new Guid(SecondCourseId),
                Name = "Second",
                Description = "Second course description",
            };

        public static CourseModel ThirdCourse = new CourseModel
            {
                Id = new Guid(ThirdCourseId),
                Name = "Third",
                Description = "Third course description"
            };

        public static CourseModel ForthCourse = new CourseModel
            {
                Id = new Guid(ForthCourseId),
                Name = "Forth",
                Description = "Forth course description"
            };

        public static CourseModel CourseToUpdate = new CourseModel
            {
                Id = new Guid(CourseToUpdateId),
                Name = "Updatable",
                Description = "Updatable course description"
            };

        public static CourseModel CourseToDelete = new CourseModel
            {
                Id = new Guid(CourseToDeleteId),
                Name = "Deletable",
                Description = "Deletable course description"
            };

        public static void SetupFixtureDataInDatabase(IDbProvider dbProvider)
        {
            dbProvider.Create(UpdateBaseFields(FirstCourse));
            dbProvider.Create(UpdateBaseFields(SecondCourse));
            dbProvider.Create(UpdateBaseFields(ThirdCourse));
            dbProvider.Create(UpdateBaseFields(ForthCourse));
            dbProvider.Create(UpdateBaseFields(CourseToDelete));
            dbProvider.Create(UpdateBaseFields(CourseToUpdate));
        }
    }
}