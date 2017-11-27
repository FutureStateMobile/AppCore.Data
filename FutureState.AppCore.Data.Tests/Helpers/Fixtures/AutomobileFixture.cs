using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Fixtures
{
    public class AutomobileFixture : FixtureBase
    {
        public static AutomobileModel GetCar()
        {
            return new AutomobileModel
            {
                Vin = "FEAB8F4C-72EA-4206-8473-15852772204B",
                WheelCount = 4,
                VehicleType = "Car"
            };
        }
        public static AutomobileModel GetMotorcycle()
        {
            return new AutomobileModel
            {
                Vin = "85AD5224-074C-4EB6-A778-A8C5ED2E24EC",
                WheelCount = 2,
                VehicleType = "Motorcycle"
            };
        }
    }
}