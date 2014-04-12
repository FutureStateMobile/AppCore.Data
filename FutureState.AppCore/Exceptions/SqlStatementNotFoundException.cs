using System;

namespace FutureState.AppCore.Data.Exceptions
{
    public class SqlStatementNotFoundException : Exception
    {
        private const string CouldNotFindFile = "Could not find the resource: '{0}'.";

        public SqlStatementNotFoundException(string fullFileName) : base(string.Format(CouldNotFindFile, fullFileName))
        {
        }
    }
}