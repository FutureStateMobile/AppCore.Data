using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IDbMapper<TMapTo> where TMapTo : class, new()
    {
        IList<string> FieldNames { get; }
        IDictionary<string, object> BuildDbParametersFrom(TMapTo model);
        TMapTo BuildFrom(IDbReader dbReader);
        IEnumerable<TMapTo> BuildListFrom(IDbReader reader);
    }
}