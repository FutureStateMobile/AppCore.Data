using System;

namespace FutureState.AppCore.Data.Exceptions
{
    public class RepositoryCreateFailedException : Exception
    {
        public RepositoryCreateFailedException(string message) : base(message)
        {
        }
    }
}