using System;
using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Fixtures
{
    public class GooseFixture : FixtureBase
    {
        public static GooseModel GetFirstGoose()
        {
            return new GooseModel
            {
                Id = Guid.NewGuid(),
                Name = "FirstGoose",
            };
        }

        public static GooseModel GetGooseToUpdate()
        {
            return new GooseModel
            {
                Id = Guid.NewGuid(),
                Name = "GooseToUpdate",
            };
        }

        public static GooseModel GetGooseToDelete()
        {
            return new GooseModel
            {
                Id = Guid.NewGuid(),
                Name = "GooseToDelete",
            };
        }
    }
}