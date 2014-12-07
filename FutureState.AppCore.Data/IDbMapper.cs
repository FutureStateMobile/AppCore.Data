using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IDbMapper<TMapTo> where TMapTo : class, new()
    {
        IDictionary<string, object> BuildDbParametersFrom(TMapTo model);
        IList<string> GetFieldNameList();

        IList<TMapTo> BuildListFrom(IDbReader reader);
        TMapTo BuildFrom(IDbReader dbReader);
    }
}