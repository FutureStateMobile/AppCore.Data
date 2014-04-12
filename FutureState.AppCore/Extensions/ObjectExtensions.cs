using System;

namespace FutureState.AppCore.Data.Extensions
{
    public static class ObjectExtensions
    {
        public static bool HasAttribute<TAttribute>(this object caller) where TAttribute : Attribute
        {
            return caller.GetType().IsDefined(typeof (TAttribute), true);
        }
    }
}