using System;
using FutureState.AppCore.Data.Tests.Helpers.Models;

namespace FutureState.AppCore.Data.Tests.Helpers.Fixtures
{
    public class GooseFixture : FixtureBase
    {
        public const string FirstGooseId = "6DB6BA38-9B01-4A64-A227-CC69A51BF57B";
        public const string GooseToUpdateId = "486652C0-7E6E-43D3-A9F3-950B66BFFCC6";
        public const string GooseToDeleteId = "3C1F3E58-8CFD-4FB9-A5D5-FCCDFFFDB73B";

        public static GooseModel FirstGoose = new GooseModel
        {
            Id = new Guid( FirstGooseId ),
            Name = "FirstGoose",
        };

        public static GooseModel GooseToUpdate = new GooseModel
        {
            Id = new Guid( GooseToUpdateId ),
            Name = "GooseToUpdate",
        };

        public static GooseModel GooseToDelete = new GooseModel
        {
            Id = new Guid( GooseToDeleteId ),
            Name = "GooseToDelete",
        };

        public static void SetupFixtureDataInDatabase ( IDbProvider dbProvider )
        {
            dbProvider.Create(FirstGoose);
            dbProvider.Create(GooseToUpdate);
            dbProvider.Create(GooseToDelete);
        }
    }
}