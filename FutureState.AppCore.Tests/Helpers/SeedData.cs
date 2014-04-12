using FutureState.AppCore.Data;
using FutureState.AppCore.Tests.Helpers.Fixtures;

namespace FutureState.AppCore.Tests.Helpers
{
    public static class SeedData
    {
         public static void SetupFixtureDataInDatabase(IDbProvider dbProvider)
         {
             CourseFixture.SetupFixtureDataInDatabase(dbProvider);
             StudentFixture.SetupFixtureDataInDatabase(dbProvider);
             BookFixture.SetupFixtureDataInDatabase(dbProvider);
         }
    }
}