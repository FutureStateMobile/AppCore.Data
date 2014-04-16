using System;

namespace FutureState.AppCore.Data.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime MinSqlValue
        {
            get { return DateTime.Parse("1/1/1753 12:00:00 AM"); }
        }
    }
}