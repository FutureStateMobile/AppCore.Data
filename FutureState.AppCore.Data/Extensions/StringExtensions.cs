using FutureState.AppCore.Data.Helpers;

namespace FutureState.AppCore.Data.Extensions
{
    internal static class StringExtensions
    {
        public static string BuildTableName(this string value)
        {
            return value.Replace( "Model", "" ).Pluralize();
        }
    }
}
