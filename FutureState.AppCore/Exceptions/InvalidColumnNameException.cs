using System;

namespace FutureState.AppCore.Data.Exceptions
{
    public class InvalidColumnNameException : Exception
    {
        public InvalidColumnNameException()
        {
        }

        public InvalidColumnNameException(string message) : base(message)
        {
        }
    }
}