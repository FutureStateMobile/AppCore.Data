using System.Diagnostics;
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
            var actualAuthor = db.Query<AuthorModel>().Where(a => a.Email == expectedAuthor.Email).SingleOrDefault();
            actualAuthor.Should().NotBeNull();
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor);

            // Update
            expectedAuthor.FirstName = "Bob";
            expectedAuthor.LastName = "Jones";
            db.Update(expectedAuthor);

            // Assert Update
            actualAuthor = db.Query<AuthorModel>().Where(a => a.Email == expectedAuthor.Email).SingleOrDefault();
            actualAuthor.Should().NotBeNull();
            actualAuthor.ShouldBeEquivalentTo(expectedAuthor);

            // Delete
            db.Delete<AuthorModel>( x => x.Id == actualAuthor.Id);

            // Assert Delete
            actualAuthor = db.Query<AuthorModel>().Where(a => a.Email == expectedAuthor.Email).SingleOrDefault();
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
            var actualCar = db.Query<AutomobileModel>().Where(c => c.Vin == car.Vin).Single();
            actualCar.Should().NotBeNull();
            actualCar.ShouldBeEquivalentTo(car);

            // update
            car.WheelCount = 6;
            car.VehicleType = "Argo";
            db.Update(car);

            // assert update
            actualCar = db.Query<AutomobileModel>().Where(c => c.Vin == car.Vin).Single();
            actualCar.Should().NotBeNull();
            actualCar.ShouldBeEquivalentTo(actualCar);
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Create_Or_Update_With_Different_Primary_Key(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // create
            var motorcycle = AutomobileFixture.GetMotorcycle();
            db.CreateOrUpdate(motorcycle);

            // assert create
            var actualMotorcycle = db.Query<AutomobileModel>().Where(c => c.Vin == motorcycle.Vin).Single();
            actualMotorcycle.Should().NotBeNull();
            actualMotorcycle.ShouldBeEquivalentTo(motorcycle);

            // upsert
            motorcycle.VehicleType = "Scooter";
            db.CreateOrUpdate(motorcycle);

            // assert update
            actualMotorcycle = db.Query<AutomobileModel>().Where(c => c.Vin == motorcycle.Vin).Single();
            actualMotorcycle.Should().NotBeNull();
            actualMotorcycle.ShouldBeEquivalentTo(actualMotorcycle);
            actualMotorcycle.VehicleType.Should().Be(motorcycle.VehicleType);
        }
    }
}