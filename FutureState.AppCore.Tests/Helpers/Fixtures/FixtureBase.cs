using System;
using FutureState.AppCore.Tests.Helpers.Models;

namespace FutureState.AppCore.Tests.Helpers.Fixtures
{
    public abstract class FixtureBase
    {
        public static TModel UpdateBaseFields<TModel> ( TModel model ) where TModel : ModelBase
        {
            var date = DateTime.UtcNow;
            model.CreatedDate = date;
            model.UpdatedDate = date;
            model.IsDeleted = false;

            return model;
        }
    }
}