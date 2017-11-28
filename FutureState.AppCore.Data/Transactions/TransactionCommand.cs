using System.Collections.Generic;

namespace FutureState.AppCore.Data.Transactions
{
    internal struct TransactionCommand
    {
        public TransactionCommand(string commandText, IEnumerable<KeyValuePair<string, object>> commandParameters)
        {
            CommandText = commandText;
            CommandParameters = commandParameters;
        }

        public IEnumerable<KeyValuePair<string,object>> CommandParameters { get; }
        public string CommandText { get; }
    }
}