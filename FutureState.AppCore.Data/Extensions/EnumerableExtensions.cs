using System;
using System.Collections.Generic;

namespace FutureState.AppCore.Data.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<TInput>( this IEnumerable<TInput> collection, Action<TInput> action )
        {
            foreach (var item in collection)
            {
                action( item );
            }
        }
    }
}
