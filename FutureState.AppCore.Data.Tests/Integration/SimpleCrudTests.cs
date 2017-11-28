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
            db.Delete<AuthorModel>( x => x.Id == expectedAuthor.Id);

            // Assert Delete
            actualAuthor = db.Query<AuthorModel>().Where(a => a.Email == expectedAuthor.Email).SingleOrDefault();
            actualAuthor.Should().BeNull();
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Do_Crud_On_Simple_Model_Object_With_Different_Primary_Key(IDbProvider db)
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

            // delete
            db.Delete<AutomobileModel>(c => c.Vin == car.Vin);

            // assert delete
            actualCar = db.Query<AutomobileModel>().Where(c => c.Vin == car.Vin).SingleOrDefault();
            actualCar.Should().BeNull();
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

            // delete
            db.Delete<AutomobileModel>(c => c.Vin == motorcycle.Vin);

            // assert delete
            actualMotorcycle = db.Query<AutomobileModel>().Where(c => c.Vin == motorcycle.Vin).SingleOrDefault();
            actualMotorcycle.Should().BeNull();
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Do_CUD_In_Transactions(IDbProvider db)
        {
            Trace.WriteLine(TraceObjectGraphInfo(db));

            // create
            var motorcycle = AutomobileFixture.GetMotorcycle();
            var car = AutomobileFixture.GetCar();
            db.RunInTransaction(transaction =>
            {
                transaction.Create(motorcycle); // create only
                transaction.CreateOrUpdate(car); // create or update
            });

            // assert create
            var actualMotorcycle = db.Query<AutomobileModel>().Where(a => a.Vin == motorcycle.Vin).SingleOrDefault();
            var actualCar = db.Query<AutomobileModel>().Where(a => a.Vin == car.Vin).SingleOrDefault();
            actualMotorcycle.Should().NotBeNull();
            actualCar.Should().NotBeNull();

            // update
            motorcycle.VehicleType = "scooter";
            car.VehicleType = "truck";
            db.RunInTransaction(transaction =>
            {
                transaction.CreateOrUpdate(motorcycle); // create or update
                transaction.Update(car); // update only
            });

            // assert update
            actualMotorcycle = db.Query<AutomobileModel>().Where(a => a.Vin == motorcycle.Vin).SingleOrDefault();
            actualCar = db.Query<AutomobileModel>().Where(a => a.Vin == car.Vin).SingleOrDefault();
            actualMotorcycle.Should().NotBeNull();
            actualCar.Should().NotBeNull();
            actualMotorcycle.VehicleType.Should().Be(motorcycle.VehicleType);
            actualCar.VehicleType.Should().Be(car.VehicleType);

            // delete
            db.RunInTransaction(transaction =>
            {
                transaction.Delete<AutomobileModel>(a=>a.Vin == motorcycle.Vin);
                transaction.Delete<AutomobileModel>(a => a.Vin == car.Vin);
            });

            // assert delete
            actualMotorcycle = db.Query<AutomobileModel>().Where(a => a.Vin == motorcycle.Vin).SingleOrDefault();
            actualCar = db.Query<AutomobileModel>().Where(a => a.Vin == car.Vin).SingleOrDefault();
            actualMotorcycle.Should().BeNull();
            actualCar.Should().BeNull();
        }

        [Test, TestCaseSource(nameof(DbProviders))]
        public void Should_Perform_Faster_When_Run_In_Transaction(IDbProvider db)
        {
            // setup
            var carWatch = new Stopwatch();
            var bikeWatch = new Stopwatch();

            // transaction test
            var car = AutomobileFixture.GetCar();
            carWatch.Start();
            db.RunInTransaction(trans =>
            {
                for (var i = 10; i < 1000; i++) // 990 records
                {
                    car.Vin = i.ToString();
                    trans.CreateOrUpdate(car);
                }
            });
            carWatch.Stop();

            // non transaction test
            var motorcycle = AutomobileFixture.GetMotorcycle();
            bikeWatch.Start();
            for (var i = 1010; i < 2000; i++) // 990 records
            {
                motorcycle.Vin = i.ToString();
                db.CreateOrUpdate(motorcycle);
            }
            bikeWatch.Stop();
            carWatch.ElapsedTicks.Should().BeLessThan(bikeWatch.ElapsedTicks);
           
            // assert record count
            var vehicleCount = db.Query<AutomobileModel>().ToList().Count;
            vehicleCount.Should().Be(1980);

            Trace.WriteLine($"Non Transaction: {bikeWatch.Elapsed.ToString(@"hh\:mm\:ss")} \t(Ticks {bikeWatch.ElapsedTicks})");
            Trace.WriteLine($"Transaction: {carWatch.Elapsed.ToString(@"hh\:mm\:ss")} \t\t(Ticks {carWatch.ElapsedTicks})");
        }
    }
}