namespace FutureState.AppCore.Data.Constraints
{
    public class CompositeUniqueConstraint : IConstraint
    {
        private readonly IDialect _dialect;
        private readonly string _key1;
        private readonly string _key2;

        public CompositeUniqueConstraint(IDialect dialect, string key1, string key2)
        {
            _dialect = dialect;
            _key1 = key1;
            _key2 = key2;
        }

        public override string ToString()
        {
            return string.Format(_dialect.CompositeUniqueConstraint, _key1, _key2);
        }
    }
}