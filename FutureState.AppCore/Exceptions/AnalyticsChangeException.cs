using System;

namespace FutureState.AppCore.Data.Exceptions
{
    public class AnalyticsChangeException : Exception
    {
        public AnalyticsChangeException()
        {
        }

        public AnalyticsChangeException(string message) : base(message)
        {
        }
    }
}