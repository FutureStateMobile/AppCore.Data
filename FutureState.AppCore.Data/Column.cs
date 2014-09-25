using System;
using System.Collections.Generic;
using FutureState.AppCore.Data.Constraints;
using FutureState.AppCore.Data.Exceptions;

namespace FutureState.AppCore.Data
{
    public class Column
    {
        public readonly IList<IConstraint> Constraints;
        public readonly string Name;
        public readonly int Precision;
        public readonly Type Type;

        private readonly IDialect _dialect;
        private readonly string _tableName;
        public static IList<KeyValuePair<Type, string>> CustomTypes = new List<KeyValuePair<Type, string>>();

        public Column(IDialect dialect, string name, Type type, string tableName, int precision)
        {
            Constraints = new List<IConstraint>();
            Name = name;
            Precision = precision;
            Type = type;
            _dialect = dialect;
            _tableName = tableName;
        }

        public Column PrimaryKey(IndexType indexType = IndexType.NonClustered)
        {
            Constraints.Add(new PrimaryKeyConstraint(_dialect, indexType));
            return this;
        }

        /// <summary>
        ///     Creates a Foreign Key Constraint.
        /// </summary>
        /// <param name="referenceTable">The reference table.</param>
        /// <param name="referenceField">The reference field.</param>
        /// <returns>Column.</returns>
        public Column ForeignKey(string referenceTable, string referenceField)
        {
            Constraints.Add(new ForeignKeyConstraint(_dialect, _tableName, Name, referenceTable, referenceField));
            Constraints.Add(new OnDeleteNoActionConstraint(_dialect));
            Constraints.Add(new OnUpdateNoActionConstraint(_dialect));
            return this;
        }

        public Column NotNullable()
        {
            Constraints.Add(new NotNullableConstraint(_dialect));
            return this;
        }

        public Column NotNullable<T>(T defaultValue)
        {
            Constraints.Add(new NotNullableConstraint(_dialect));
            Constraints.Add(new DefaultConstraint<T>(_dialect, defaultValue));
            return this;
        }

        public Column Nullable()
        {
            Constraints.Add(new NullableConstraint(_dialect));
            return this;
        }

        public Column Unique()
        {
            Constraints.Add(new UniqueConstraint(_dialect));
            return this;
        }

        public Column Default<T>(T defaultValue)
        {
            Constraints.Add(new DefaultConstraint<T>(_dialect, defaultValue));
            return this;
        }

        public Column AsCustomType(string dialectValue)
        {
            CustomTypes.Add(new KeyValuePair<Type, string>(Type, dialectValue));
            return this;
        }

        public override string ToString()
        {
            return Environment.NewLine +
                    string.Format( _dialect.CreateColumn, Name, GetDataType( Type, Precision ),
                                    string.Join( " ", Constraints ) );
        }

        private string GetDataType(Type type, int precision)
        {
            if (type == typeof (String) && precision == 0)
                return _dialect.MaxString;

            if (type == typeof (String))
                return string.Format(_dialect.LimitedString, precision);

            if (type == typeof (int) || type == typeof (Int32))
                return _dialect.Integer;

            if (type == typeof (Int16))
                return _dialect.Int16;

            if (type == typeof (DateTime))
                return _dialect.DateTime;

            if (type == typeof (Guid))
                return _dialect.Guid;

            if (type == typeof (bool))
                return _dialect.Bool;

            if (type == typeof (decimal))
                return _dialect.Decimal;

            if (type == typeof (byte))
                return _dialect.Byte;

            if (type == typeof (Int64))
                return _dialect.Int64;

            if (type == typeof (double) || type == typeof (float))
                return _dialect.Double;

            if (type == typeof (byte[]))
                return _dialect.ByteArray;

            if (type == typeof (Single))
                return _dialect.Single;

            if (type == typeof (TimeSpan))
                return _dialect.TimeSpan;

            foreach (var customType in CustomTypes)
            {
                if (type == customType.Key)
                {
                    return customType.Value;
                }
            }

            throw new DataTypeNotSupportedException();
        }
    }
}