using System;

namespace FutureState.AppCore.Data.Constraints
{
    public class NonClusteredConstraint : IConstraint
    {
        private readonly IDialect _dialect;

        public NonClusteredConstraint(IDialect dialect)
        {
            _dialect = dialect;
        }

        public override string ToString()
        {
            return _dialect.NonClusteredConstraint;
        }
    }
}