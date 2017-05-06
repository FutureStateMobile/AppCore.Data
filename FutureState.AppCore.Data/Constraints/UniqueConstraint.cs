namespace FutureState.AppCore.Data.Constraints
{
    public class UniqueConstraint : IConstraint
    {
        private static IDialect _dialect;

        public UniqueConstraint(IDialect dialect)
        {
            _dialect = dialect;
        }

        public override string ToString() => _dialect.UniqueConstraint;
    }
}