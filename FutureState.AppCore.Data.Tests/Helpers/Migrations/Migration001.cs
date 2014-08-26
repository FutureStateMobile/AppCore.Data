using System;
using System.Collections.Generic;
using FutureState.AppCore.Data.Tests.Helpers.Fixtures;
using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Migrations
{
    public class Migration001 : Migration
    {
        public const string BobEmail = "bob@futurestatemobile.com";
        public static readonly Guid StudentBobId = new Guid("427A1F2D-2D1A-4492-BD2C-2CF569C46FBB");
        public static readonly Guid StudentJillId = new Guid("6C83DDEC-5E58-4F28-BDE2-61EBF1B49691");
        public static readonly Guid MathCourseId = new Guid("0F9D7D41-BBCA-4663-873C-AE2B5F31BEA4");
        public static readonly Guid EnglishCourseId = new Guid("78CB8CFD-0A09-4385-ABFE-38B4A087220A");
        public static string JillEmail = "jill@futurestatemobile.com";

        public Migration001()
        {
            MigrationVersion = 1;
        }

        public override void Migrate()
        {
            var migration = new DbMigration(DbProvider.Dialect);
            var database = new Database(DbProvider.DatabaseName, DbProvider.Dialect);

            var studentTable = database.AddTable("Students");
            studentTable.AddColumn("Id", typeof (Guid)).PrimaryKey().NotNullable();
            studentTable.AddColumn("FirstName", typeof (string), 100);
            studentTable.AddColumn("LastName", typeof (string), 100);
            studentTable.AddColumn("Email", typeof (string), 100).NotNullable().Unique();
            studentTable.AddColumn("CreatedDate", typeof (DateTime)).NotNullable();
            studentTable.AddColumn("UpdatedDate", typeof (DateTime)).NotNullable();
            studentTable.AddColumn("IsDeleted", typeof (bool)).NotNullable(false);

            var courseTable = database.AddTable("Courses");
            courseTable.AddColumn("Id", typeof (Guid)).PrimaryKey().NotNullable();
            courseTable.AddColumn("Name", typeof (string), 100).NotNullable();
            courseTable.AddColumn("Description", typeof (string), 100).NotNullable();
            courseTable.AddColumn("CreatedDate", typeof (DateTime)).NotNullable();
            courseTable.AddColumn("UpdatedDate", typeof (DateTime)).NotNullable();
            courseTable.AddColumn("IsDeleted", typeof (bool)).NotNullable(false);


            // OneToMany Relationship to Student
            var bookTable = database.AddTable("Books");
            bookTable.AddColumn("Id", typeof (Guid)).PrimaryKey().NotNullable();
            bookTable.AddColumn("StudentId", typeof(Guid)).NotNullable().ForeignKey("Students", "Id");
            bookTable.AddColumn("Name", typeof(string), 100).NotNullable();
            bookTable.AddColumn("BookNumber", typeof(int)).Nullable();
            bookTable.AddColumn("PublishDate", typeof (DateTime)).NotNullable();
            bookTable.AddColumn("CreatedDate", typeof (DateTime)).NotNullable();
            bookTable.AddColumn("UpdatedDate", typeof (DateTime)).NotNullable();
            bookTable.AddColumn("IsDeleted", typeof (bool)).NotNullable(false);

            var gooseTable = database.AddTable("Geese");
            gooseTable.AddColumn("Id", typeof (Guid)).PrimaryKey().NotNullable();
            gooseTable.AddColumn("Name", typeof (string), 100).Nullable();

            // ManyToMany Join Tables are currently handled under the covers (without an associated model)
            // Naming convention used by ORM is to joing the 2 table names together in alphabetical order
            var courseStudentTable = database.AddTable("Courses_Students").CompositeKey("StudentId", "CourseId");
            courseStudentTable.AddColumn("CourseId", typeof (Guid)).ForeignKey("Courses", "Id").NotNullable();
            courseStudentTable.AddColumn("StudentId", typeof (Guid)).ForeignKey("Students", "Id").NotNullable();

            DbProvider.ExecuteNonQuery(migration.GenerateDDL(database));
        }

        public override void ServerAfterMigrate()
        {
            // Create some base data
            var mathCourse = new CourseModel
                {
                    Name = "Math 101",
                    Id = MathCourseId,
                    Description = "This is a math class",
                };

            var englishCourse = new CourseModel
                {
                    Id = EnglishCourseId,
                    Name = "English 101",
                    Description = "This is an english course.",
                };

            DbProvider.Create(FixtureBase.UpdateBaseFields(englishCourse));
            DbProvider.Create(FixtureBase.UpdateBaseFields(mathCourse));

            // Add the users (depends on roles)
            var jill = new StudentModel
                {
                    Id = StudentJillId,
                    FirstName = "Jill",
                    LastName = "",
                    Email = JillEmail,
                    Courses = new List<CourseModel> {englishCourse},
                };

            var bob = new StudentModel
                {
                    Id = StudentBobId,
                    FirstName = "Bob",
                    LastName = "",
                    Email = BobEmail,
                    Courses = new List<CourseModel> {mathCourse},
                };
            DbProvider.Create(FixtureBase.UpdateBaseFields(jill));
            DbProvider.Create(FixtureBase.UpdateBaseFields(bob));
        }
    }
}