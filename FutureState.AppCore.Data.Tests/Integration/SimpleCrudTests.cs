using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using FutureState.AppCore.Data.Tests.Helpers.Fixtures;
using FutureState.AppCore.Data.Tests.Helpers.Models;
using NUnit.Framework;

namespace FutureState.AppCore.Data.Tests.Integration
{
    [TestFixture]
    public class SimpleCrudTests : IntegrationTestBase
    {
        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Do_Crud_On_Simple_Model_Object(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // Create
            var expectedAuthor = AuthorFixture.GetFirstAuthor();
            db.Create(expectedAuthor);

            // Assert Create
            var actualAuthor = db.Query<AuthorModel>().Where(a => a.Email == expectedAuthor.Email).Select().SingleOrDefault();
            actualAuthor.Should().NotBeNull();
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor);

            // Update
            expectedAuthor.FirstName = "Bob";
            expectedAuthor.LastName = "Jones";
            db.Update(expectedAuthor);

            // Assert Update
            actualAuthor = db.Query<AuthorModel>().Where(a => a.Email == expectedAuthor.Email).Single();
            actualAuthor.Should().NotBeNull();
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor);

            // Delete
            db.Delete<AuthorModel>( x => x.Id == actualAuthor.Id);

            // Assert Delete
            actualAuthor = db.Query<AuthorModel>().Where(a => a.Email == expectedAuthor.Email).Select().SingleOrDefault();
            actualAuthor.Should().BeNull();
        }

        [Test, TestCaseSource(nameof(DbProviders))]

        public void Should_Update_With_Different_Primary_Key(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // create
            var car = AutomobileFixture.GetCar();
            db.Create(car);

            // assert create
            var actualCar = db.Query<AutomobileModel>().Where(c => c.Vin == car.Vin).SingleOrDefault();
            actualCar.Should().NotBeNull();
            actualCar.ShouldBeEquivalentTo(car);

            // update
            car.WheelCount = 6;
            car.VehicleType = "Argo";
            db.Update(car);

            // assert update
            actualCar = db.Query<AutomobileModel>().Where(c => c.Vin == car.Vin).SingleOrDefault();
            actualCar.Should().NotBeNull();
            actualCar.ShouldBeEquivalentTo(actualCar);
        }
    }
}