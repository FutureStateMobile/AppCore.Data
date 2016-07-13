using System;
using FutureState.AppCore.Data.Tests.Helpers.Fixtures;
using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Migrations
{
    public class Migration001 : AppCoreMigration
    {
        public static readonly Guid TheHobbitId = new Guid("E4BA6C4A-92BC-4B53-A833-680CEC3686DB");
        public static readonly Guid TheScrewTapeLettersId = new Guid("4C65C4BD-A87D-46E8-A39E-9B4D85078D52");
        public static readonly Guid TheLionWitchWardrobeId = new Guid("4ED8ABDC-706A-4F02-9B02-7E8E6BEBD774");

        public static readonly Guid AuthorCSLewisId = new Guid("427A1F2D-2D1A-4492-BD2C-2CF569C46FBB");
        public const string CSLewisEmail = "cs@futurestatemobile.com";
        public static readonly Guid AuthorJRTolkienId = new Guid("6C83DDEC-5E58-4F28-BDE2-61EBF1B49691");
        public static string JRTolkien = "jr@futurestatemobile.com";

        public static readonly Guid BobsPublishingId = new Guid("0F9D7D41-BBCA-4663-873C-AE2B5F31BEA4");
        public static readonly Guid RandomHouseId = new Guid("78CB8CFD-0A09-4385-ABFE-38B4A087220A");

        public Migration001():base(1)
        {
            Migration[MigrationStep.Migrate] = (database, dbProvider) =>
            {
                var studentTable = database.AddTable("Authors");
                studentTable.AddColumn("Id", typeof(Guid)).PrimaryKey().Clustered().NotNullable();
                studentTable.AddColumn("FirstName", typeof(string), 100);
                studentTable.AddColumn("LastName", typeof(string), 100);
                studentTable.AddColumn("Email", typeof(string), 100).NotNullable().Unique();
                studentTable.AddColumn("CreatedDate", typeof(DateTime)).NotNullable();
                studentTable.AddColumn("UpdatedDate", typeof(DateTime)).NotNullable();
                studentTable.AddColumn("IsDeleted", typeof(bool)).NotNullable(false);

                var courseTable = database.AddTable("Publishers");
                courseTable.AddColumn("Id", typeof(Guid)).PrimaryKey().NotNullable();
                courseTable.AddColumn("Name", typeof(string), 100).NotNullable();
                courseTable.AddColumn("Description", typeof(string), 100).NotNullable();
                courseTable.AddColumn("CreatedDate", typeof(DateTime)).NotNullable();
                courseTable.AddColumn("UpdatedDate", typeof(DateTime)).NotNullable();
                courseTable.AddColumn("IsDeleted", typeof(bool)).NotNullable(false);

                // OneToMany Relationship to Student
                var bookTable = database.AddTable("Books");
                bookTable.AddColumn("Id", typeof(Guid)).PrimaryKey().NotNullable();
                bookTable.AddColumn("PublisherId", typeof(Guid)).Nullable().ForeignKey("Publishers", "Id");
                bookTable.AddColumn("Name", typeof(string), 100).NotNullable();
                bookTable.AddColumn("ISBN", typeof(int)).Nullable();
                bookTable.AddColumn("PublishDate", typeof(DateTime)).NotNullable();
                bookTable.AddColumn("CreatedDate", typeof(DateTime)).NotNullable();
                bookTable.AddColumn("UpdatedDate", typeof(DateTime)).NotNullable();
                bookTable.AddColumn("IsDeleted", typeof(bool)).NotNullable(false);

                // ManyToMany Join Tables are currently handled under the covers (without an associated model)
                // Naming convention used by ORM is to joing the 2 table names together in alphabetical order
                var courseStudentTable = database.AddTable("Authors_Books").CompositeKey("AuthorId", "BookId", ClusterType.Clustered);
                courseStudentTable.AddColumn("BookId", typeof(Guid)).ForeignKey("Books", "Id").NotNullable();
                courseStudentTable.AddColumn("AuthorId", typeof(Guid)).ForeignKey("Authors", "Id").NotNullable();

                // Example of the inflection library at work
                var gooseTable = database.AddTable("Geese");
                gooseTable.AddColumn("Id", typeof(Guid)).PrimaryKey().NotNullable();
                gooseTable.AddColumn("Name", typeof(string), 100).Nullable();
            };

            Migration[MigrationStep.ServerAfterMigrate] = (database, dbProvider )=>
            {
                // Create some base data
                var bobsPublishing = new PublisherModel
                {
                    Id = BobsPublishingId,
                    Name = "Bob's publishing",
                    Description = "This is bobs publishing company.",
                };

                var randomHouse = new PublisherModel
                {
                    Id = RandomHouseId,
                    Name = "Random House",
                    Description = "This is a Canadian book publisher.",
                };

                dbProvider.Create(FixtureBase.UpdateBaseFields(randomHouse));
                dbProvider.Create(FixtureBase.UpdateBaseFields(bobsPublishing));

                var theHobbit = new BookModel
                {
                    Id = TheHobbitId,
                    ISBN = 4444,
                    Publisher = randomHouse,
                    Name = "The Hobbit"
                };

                var screwTapeLetters = new BookModel
                {
                    Id = TheScrewTapeLettersId,
                    ISBN = 1234,
                    Publisher = randomHouse,
                    Name = "The Screwtape Letters"
                };

                var theLionWitchWardrobe = new BookModel
                {
                    Id = TheLionWitchWardrobeId,
                    ISBN = 1234,
                    Publisher = randomHouse,
                    Name = "The Lion the Witch and the Wardrobe"
                };

                dbProvider.Create(FixtureBase.UpdateBaseFields(theHobbit));
                dbProvider.Create(FixtureBase.UpdateBaseFields(screwTapeLetters));
                dbProvider.Create(FixtureBase.UpdateBaseFields(theLionWitchWardrobe));

                var jrTolkien = new AuthorModel
                {
                    Id = AuthorJRTolkienId,
                    FirstName = "JR",
                    LastName = "Tolkien",
                    Email = JRTolkien,
                };

                jrTolkien.AddBooks(theHobbit);

                var csLewis = new AuthorModel
                {
                    Id = AuthorCSLewisId,
                    FirstName = "CS",
                    LastName = "Lewis",
                    Email = CSLewisEmail,
                };
                csLewis.AddBooks(screwTapeLetters, theLionWitchWardrobe);

                dbProvider.Create(FixtureBase.UpdateBaseFields(jrTolkien));
                dbProvider.Create(FixtureBase.UpdateBaseFields(csLewis));
            };
        }
    }
}