using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IModelMapper<TMapTo> where TMapTo : class, new()
    {
        IDictionary<string, object> BuildDbParametersFrom(TMapTo model);
        IList<string> GetFieldNameList();

        IList<TMapTo> BuildListFrom(IDbReader reader);
        TMapTo BuildFrom(IDbReader dbReader);

        IList<TMapTo> BuildListFrom<TInput>(IList<TInput> input) where TInput : class;
        TMapTo BuildFrom<TInput>(TInput input) where TInput : class;
    }
}