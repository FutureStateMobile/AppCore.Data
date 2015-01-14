using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IModelMapper<TMapTo, in TMapFrom>
        where TMapTo : class, new()
        where TMapFrom : class, new()
    {
        IEnumerable<TMapTo> BuildListFrom(IEnumerable<TMapFrom> input);
        TMapTo BuildFrom(TMapFrom input);
        TMapTo BuildFrom(TMapFrom input, TMapTo output);
    }
}