using FutureState.AppCore.Data.Tests.Helpers.Fixtures;

namespace FutureState.AppCore.Data.Tests.Helpers
{
    public static class SeedData
    {
        public static void SetupFixtureDataInDatabase(IDbProvider dbProvider)
        {
            CourseFixture.SetupFixtureDataInDatabase(dbProvider);
            StudentFixture.SetupFixtureDataInDatabase(dbProvider);
            BookFixture.SetupFixtureDataInDatabase(dbProvider);
            GooseFixture.SetupFixtureDataInDatabase(dbProvider);
        }
    }
}