using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IDbMapper<TMapTo> where TMapTo : class, new()
    {
        IList<string> GetFieldNames();
        IDictionary<string, object> BuildDbParametersFrom(TMapTo model);
        IList<TMapTo> BuildListFrom(IDbReader reader);
        TMapTo BuildFrom(IDbReader dbReader);
    }
}