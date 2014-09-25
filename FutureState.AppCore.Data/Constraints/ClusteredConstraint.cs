using System;

namespace FutureState.AppCore.Data.Constraints
{
    public class ClusteredConstraint : IConstraint
    {
        private readonly IDialect _dialect;

        public ClusteredConstraint(IDialect dialect)
        {
            _dialect = dialect;
        }
        public override string ToString()
        {
            return _dialect.ClusteredConstraint;
        }
    }
}