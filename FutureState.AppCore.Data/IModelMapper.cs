using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IModelMapper<TMapTo> where TMapTo : class, new()
    {
        IList<TMapTo> BuildListFrom<TInput> ( IList<TInput> input ) where TInput : class;
        TMapTo BuildFrom<TInput> ( TInput input ) where TInput : class;
    }
}