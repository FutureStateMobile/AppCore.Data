using System;

namespace FutureState.AppCore.Data.Exceptions
{
    public class NullFieldException : Exception
    {
        public NullFieldException()
        {
        }

        public NullFieldException(string message) : base(message)
        {
        }
    }
}