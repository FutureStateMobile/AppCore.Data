using System.Text.RegularExpressions;
using FutureState.AppCore.Data.Helpers;

namespace FutureState.AppCore.Data.Extensions
{
    internal static class StringExtensions
    {
        public static string BuildTableName(this string value)
        {
            var name = value.Replace("Model", "");
            name = Regex.Replace(name, @"\`\d", "");
            return name.Pluralize();
        }
    }
}
