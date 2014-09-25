namespace FutureState.AppCore.Data.Constraints
{
    public class PrimaryKeyConstraint : IConstraint
    {
        private static IDialect _dialect;
        private readonly IndexType _indexType;

        public PrimaryKeyConstraint(IDialect dialect, IndexType indexType)
        {
            _dialect = dialect;
            _indexType = indexType;
        }

        public override string ToString()
        {
            switch (_indexType)
            {
                case IndexType.Clustered:
                    return _dialect.ClusteredPrimaryKeyConstraint;
                default:
                    return _dialect.NonClusteredPrimaryKeyConstraint;
            }
        }
    }
}