namespace FutureState.AppCore.Data.Constraints
{
    public class NullableConstraint : IConstraint
    {
        private readonly IDialect _dialect;

        public NullableConstraint(IDialect dialect)
        {
            _dialect = dialect;
        }

        public override string ToString()
        {
            return _dialect.NullableConstraint;
        }
    }
}