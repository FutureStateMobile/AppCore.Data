namespace FutureState.AppCore.Data.Constraints
{
    public class NotNullableConstraint : IConstraint
    {
        private readonly IDialect _dialect;

        public NotNullableConstraint(IDialect dialect)
        {
            _dialect = dialect;
        }

        public override string ToString()
        {
            return _dialect.NotNullableConstraint;
        }
    }
}