namespace FutureState.AppCore.Data.Constraints
{
    public class PrimaryKeyConstraint : IConstraint
    {
        private static IDialect _dialect;

        public PrimaryKeyConstraint(IDialect dialect)
        {
            _dialect = dialect;
        }

        public override string ToString()
        {
            return _dialect.PrimaryKeyConstraint;
        }
    }
}