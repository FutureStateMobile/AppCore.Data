using System;
using FutureState.AppCore.Data.Helpers;

namespace FutureState.AppCore.Data.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetDbSafeDate(this DateTime dateTime) => dateTime < DateTimeHelper.MinSqlValue ? 
            DateTimeHelper.MinSqlValue : 
            dateTime;
    }
}