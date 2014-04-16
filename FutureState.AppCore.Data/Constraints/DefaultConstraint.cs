using System;

namespace FutureState.AppCore.Data.Constraints
{
    public class DefaultConstraint<T> : IConstraint
    {
        private readonly T _defaultValue;
        private readonly IDialect _dialect;

        public DefaultConstraint(IDialect dialect, T defaultValue)
        {
            _dialect = dialect;
            _defaultValue = defaultValue;
        }

        public override string ToString()
        {
            if (typeof (T) == typeof (int))
                return string.Format(_dialect.DefaultIntegerConstraint, _defaultValue);

            if (typeof (T) == typeof (bool))
            {
                string value = ((bool?) Convert.ChangeType(_defaultValue, typeof (T), null) ?? false) ? "1" : "0";
                return string.Format(_dialect.DefaultBoolConstraint, value);
            }

            return string.Format(_dialect.DefaultStringConstraint, _defaultValue);
        }
    }
}