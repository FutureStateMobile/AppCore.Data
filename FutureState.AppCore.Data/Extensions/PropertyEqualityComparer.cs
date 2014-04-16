using System;
using System.Collections.Generic;
using System.Linq;

namespace FutureState.AppCore.Data.Extensions
{
    public static class ComparerLinqExtensions
    {
        public static IEnumerable<TSource> Exclude<TSource, TKey>(this IEnumerable<TSource> first,
                                                                  IEnumerable<TSource> second,
                                                                  Func<TSource, TKey> keySelector,
                                                                  IEqualityComparer<TKey> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<TKey>.Default;
            var set = new HashSet<TKey>(second.Select(keySelector), comparer);
            return first.Where(item => set.Add(keySelector(item)));
        }
    }
}