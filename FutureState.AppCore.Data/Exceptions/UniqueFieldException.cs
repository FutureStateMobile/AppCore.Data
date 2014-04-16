using System;

namespace FutureState.AppCore.Data.Exceptions
{
    public class UniqueFieldException : Exception
    {
        public UniqueFieldException()
        {
        }

        public UniqueFieldException(string message) : base(message)
        {
        }
    }
}