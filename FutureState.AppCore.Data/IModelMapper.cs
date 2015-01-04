using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IModelMapper<TMapTo, TMapFrom>
        where TMapTo : class, new()
        where TMapFrom : class, new()
    {
        IList<TMapTo> BuildListFrom(IList<TMapFrom> input);
        TMapTo BuildFrom(TMapFrom input);
        TMapTo BuildFrom(TMapFrom input, TMapTo output);
    }
}