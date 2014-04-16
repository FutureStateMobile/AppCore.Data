namespace FutureState.AppCore.Data.Constraints
{
    public class ForeignKeyConstraint : IConstraint
    {
        private static IDialect _dialect;
        private readonly string _localField;
        private readonly string _localTable;
        private readonly string _referenceField;
        private readonly string _referenceTable;

        public ForeignKeyConstraint(IDialect dialect, string localTable, string localField, string referenceTable, string referenceField)
        {
            _dialect = dialect;
            _localTable = localTable;
            _localField = localField;
            _referenceTable = referenceTable;
            _referenceField = referenceField;
        }

        public override string ToString()
        {
            return string.Format(_dialect.ForeignKeyConstraint, _localTable, _localField, _referenceTable, _referenceField);
        }
    }
}