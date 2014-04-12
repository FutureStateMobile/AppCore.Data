using System;

namespace FutureState.AppCore.Data.Exceptions
{
    public class SystemAccountChangeException : Exception
    {
        public SystemAccountChangeException(string message) : base(message)
        {
        }
    }
}