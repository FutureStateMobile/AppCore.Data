namespace FutureState.AppCore.Data.Constraints
{
    public class CompositeUniqueConstraint : IConstraint
    {
        private readonly IDialect _dialect;
        private readonly string _key1;
        private readonly string _key2;
        private readonly ClusterType _clusterType;

        public CompositeUniqueConstraint(IDialect dialect, string key1, string key2, ClusterType clusterType)
        {
            _dialect = dialect;
            _key1 = key1;
            _key2 = key2;
            _clusterType = clusterType;
        }

        public override string ToString()
        {
            string cluster;
            switch (_clusterType)
            {
                case ClusterType.Clustered:
                    cluster = _dialect.ClusteredConstraint;
                    break;
                default:
                    cluster = _dialect.NonClusteredConstraint;
                    break;
            }
            return string.Format(_dialect.CompositeUniqueConstraint, _key1, _key2, cluster);
        }
    }
}