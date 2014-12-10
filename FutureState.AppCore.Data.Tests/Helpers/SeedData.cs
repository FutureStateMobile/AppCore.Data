using FutureState.AppCore.Data.Tests.Helpers.Fixtures;

namespace FutureState.AppCore.Data.Tests.Helpers
{
    public static class SeedData
    {
        public static void SetupFixtureDataInDatabase(IDbProvider dbProvider)
        {
            CourseFixture.SetupFixtureDataInDatabase(dbProvider);
            AuthorFixture.SetupFixtureDataInDatabase(dbProvider);
            BookFixture.SetupFixtureDataInDatabase(dbProvider);
            GooseFixture.SetupFixtureDataInDatabase(dbProvider);
        }
    }
}