using System.Collections.Generic;

namespace FutureState.AppCore.Data.Extensions
{
    public static class GenericNullValueComparer
    {
        public static bool IsNull<T>(this T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }
    }
}