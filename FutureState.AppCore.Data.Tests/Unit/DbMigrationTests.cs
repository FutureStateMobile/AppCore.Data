using System;
using FutureState.AppCore.Data.SqlServer;
using FutureState.AppCore.Data.Sqlite;
using FutureState.AppCore.Data.Tests.Helpers.CustomTypes;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Unit
{
    [TestFixture]
    public class DbMigrationTests
    {
        [Test]
        public void ShouldAddADefaultValueToAField()
        {
            // Setup
            const string expectedDDL = @"
CREATE TABLE [Test] (
[Id] int DEFAULT(7),
[Foo] nvarchar(max) );";

            var dialect = new SqlServerDialect();
            var migration = new DbMigration(dialect);
            var database = new Database("MyDatabase", dialect);

            var testTable = database.AddTable("Test");
            testTable.AddColumn("Id", typeof (int)).Default(7);
            testTable.AddColumn("Foo", typeof (string));

            // Execute
            var actualDDL = migration.GenerateDDL(database);

            // Assert
            Assert.AreEqual(expectedDDL, actualDDL);
        }

        [Test]
        public void ShouldUpdateAnExistingTableAndAddANewColumn ()
        {
            // Setup
            const string expectedDDL = @"
ALTER TABLE [Test] ADD 
[Name] nvarchar(100) NULL;
ALTER TABLE [Test] ADD 
[Foo] int DEFAULT(1);";

            var dialect = new SqlServerDialect();
            var migration = new DbMigration( dialect );
            var database = new Database( "MyDatabase", dialect );

            var testTable = database.UpdateTable( "Test" );
            testTable.AddColumn( "Name", typeof( string ), 100 ).Nullable();
            testTable.AddColumn( "Foo", typeof( int ) ).Default(1);

            // Execute
            var actualDDL = migration.GenerateDDL( database );

            // Assert
            Assert.AreEqual( expectedDDL, actualDDL );
        }

        [Test]
        public void ShouldBuildProperDDLForANewSqlServerDatabase()
        {
            // Setup
            const string expectedDDL = @"
CREATE TABLE [Teachers] (
[Id] uniqueidentifier PRIMARY KEY NONCLUSTERED NOT NULL,
[TeacherName] nvarchar(100) NOT NULL);
CREATE TABLE [Courses] (
[Id] uniqueidentifier PRIMARY KEY NONCLUSTERED NOT NULL,
[CourseName] nvarchar(100) NOT NULL,
[CourseDescription] nvarchar(max) ,
[CourseTeacher] uniqueidentifier CONSTRAINT FK_Courses_CourseTeacher FOREIGN KEY (CourseTeacher) REFERENCES Teachers (Id) ON DELETE NO ACTION ON UPDATE NO ACTION NOT NULL,
[IsAvailable] bit NOT NULL DEFAULT(0));";

            var dialect = new SqlServerDialect();
            var migration = new DbMigration(dialect);
            var database = new Database("MyDatabase", dialect);

            var teacher = database.AddTable("Teachers");
            teacher.AddColumn("Id", typeof (Guid)).PrimaryKey().NonClustered().NotNullable();
            teacher.AddColumn("TeacherName", typeof (String), 100).NotNullable();

            var course = database.AddTable("Courses");
            course.AddColumn("Id", typeof (Guid)).PrimaryKey().NonClustered().NotNullable();
            course.AddColumn("CourseName", typeof (String), 100).NotNullable();
            course.AddColumn("CourseDescription", typeof (String));
            course.AddColumn("CourseTeacher", typeof (Guid)).ForeignKey("Teachers", "Id").NotNullable();
            course.AddColumn("IsAvailable", typeof (bool)).NotNullable(false);

            // Execute
            var actualDDL = migration.GenerateDDL(database);

            // Assert
            Assert.AreEqual(expectedDDL, actualDDL);
        }

        [Test]
        public void ShouldBuildProperDDLForANewSqliteDatabase()
        {
            // Setup
            const string expectedDDL = @"
CREATE TABLE [Teachers] (
[Id] uniqueidentifier PRIMARY KEY NOT NULL,
[TeacherName] nvarchar(100) NOT NULL);
CREATE TABLE [Courses] (
[Id] uniqueidentifier PRIMARY KEY NOT NULL,
[CourseName] nvarchar(100) NOT NULL,
[CourseDescription] text ,
[CourseTeacher] uniqueidentifier REFERENCES Teachers (Id) ON DELETE NO ACTION ON UPDATE NO ACTION NOT NULL,
[IsAvailable] bit NOT NULL DEFAULT(0));";

            var dialect = new SqliteDialect();
            var migration = new DbMigration(dialect);
            var database = new Database("MyDatabase", dialect);

            var teacher = database.AddTable("Teachers");
            teacher.AddColumn("Id", typeof (Guid)).PrimaryKey().NotNullable();
            teacher.AddColumn("TeacherName", typeof (String), 100).NotNullable();

            var course = database.AddTable("Courses");
            course.AddColumn("Id", typeof (Guid)).PrimaryKey().NotNullable();
            course.AddColumn("CourseName", typeof (String), 100).NotNullable();
            course.AddColumn("CourseDescription", typeof (String));
            course.AddColumn("CourseTeacher", typeof (Guid)).ForeignKey("Teachers", "Id").NotNullable();
            course.AddColumn("IsAvailable", typeof (bool)).NotNullable(false);

            // Execute
            var actualDDL = migration.GenerateDDL(database);

            // Assert
            Assert.AreEqual(expectedDDL, actualDDL);
        }

        [Test]
        public void ShouldBuildTableWithCompositeKey()
        {
            // Setup
            const string expectedDDL = @"
CREATE TABLE [Roles_Permissions] (
[RoleId] uniqueidentifier CONSTRAINT FK_Roles_Permissions_RoleId FOREIGN KEY (RoleId) REFERENCES Courses (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
[PermissionId] uniqueidentifier CONSTRAINT FK_Roles_Permissions_PermissionId FOREIGN KEY (PermissionId) REFERENCES Permissions (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
CONSTRAINT PK_Roles_Permissions_Composite PRIMARY KEY NONCLUSTERED (RoleId, PermissionId));";

            var dialect = new SqlServerDialect();
            var migration = new DbMigration(dialect);
            var database = new Database("MyDatabase", dialect);

            var rolesPermissionsTable = database.AddTable("Roles_Permissions").CompositeKey("RoleId", "PermissionId");
            rolesPermissionsTable.AddColumn("RoleId", typeof (Guid)).ForeignKey("Courses", "Id");
            rolesPermissionsTable.AddColumn("PermissionId", typeof (Guid)).ForeignKey("Permissions", "Id");

            // Execute
            var actualDDL = migration.GenerateDDL(database);

            // Assert
            Assert.AreEqual(expectedDDL, actualDDL);
        }

        [Test]
        public void ShouldBuildTableWithCompositeUnique()
        {
            const string expectedDDL = @"
CREATE TABLE [Roles_Permissions] (
[RoleId] uniqueidentifier CONSTRAINT FK_Roles_Permissions_RoleId FOREIGN KEY (RoleId) REFERENCES Courses (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
[PermissionId] uniqueidentifier CONSTRAINT FK_Roles_Permissions_PermissionId FOREIGN KEY (PermissionId) REFERENCES Permissions (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
CONSTRAINT PK_RoleId_PermissionId_Composite UNIQUE NONCLUSTERED (RoleId, PermissionId));";


            var dialect = new SqlServerDialect();
            var migration = new DbMigration(dialect);
            var database = new Database("MyDatabase", dialect);

            var rolesPermissionsTable = database.AddTable("Roles_Permissions").CompositeUnique("RoleId", "PermissionId");
            rolesPermissionsTable.AddColumn("RoleId", typeof (Guid)).ForeignKey("Courses", "Id");
            rolesPermissionsTable.AddColumn("PermissionId", typeof (Guid)).ForeignKey("Permissions", "Id");

            // Execute
            var actualDDL = migration.GenerateDDL(database);

            // Assert
            Assert.AreEqual(expectedDDL, actualDDL);
        }

        [Test]
        public void ShouldCreateATableWithANotNullInt()
        {
            // Setup
            const string expectedDDL = @"
CREATE TABLE [Test] (
[Id] int NOT NULL,
[Foo] nvarchar(max) );";

            var dialect = new SqlServerDialect();
            var migration = new DbMigration(dialect);
            var database = new Database("MyDatabase", dialect);

            var testTable = database.AddTable("Test");
            testTable.AddColumn("Id", typeof (int)).NotNullable();
            testTable.AddColumn("Foo", typeof (string));

            // Execute
            var actualDDL = migration.GenerateDDL(database);

            // Assert
            Assert.AreEqual(expectedDDL, actualDDL);
        }

        [Test]
        public void ShouldCreateATableWithALatLongColumn()
        {
            // Setup
            const string expectedDDL = @"
CREATE TABLE [Test] (
[Latitude] double(9, 6) );";

            var dialect = new SqlServerDialect();
            var customDialect = new CustomDialect();
            var migration = new DbMigration(dialect);
            var database = new Database("MyDatabase", dialect);

            var testTable = database.AddTable("Test");
            testTable.AddColumn("Latitude", typeof (LatLong)).AsCustomType(customDialect.LatLong);

            // Execute
            var actualDDL = migration.GenerateDDL(database);

            // Assert
            Assert.AreEqual(expectedDDL, actualDDL);
        }
    }
}