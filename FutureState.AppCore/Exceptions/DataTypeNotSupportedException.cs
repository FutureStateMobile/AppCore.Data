using System;

namespace FutureState.AppCore.Data.Exceptions
{
    public class DataTypeNotSupportedException : Exception
    {
        public DataTypeNotSupportedException()
        {
        }

        public DataTypeNotSupportedException(string message) : base(message)
        {
        }
    }
}