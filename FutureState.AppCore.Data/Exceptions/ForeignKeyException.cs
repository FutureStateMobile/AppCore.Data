using System;

namespace FutureState.AppCore.Data.Exceptions
{
    public class ForeignKeyException : Exception
    {
        public ForeignKeyException(string message) : base(message)
        {
        }
    }
}