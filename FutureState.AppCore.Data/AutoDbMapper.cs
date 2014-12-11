using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FutureState.AppCore.Data.Attributes;
using FutureState.AppCore.Data.Extensions;
using FutureState.AppCore.Data.Helpers;

namespace FutureState.AppCore.Data
{
    public class AutoDbMapper<TMapTo> : IDbMapper<TMapTo> where TMapTo : class, new()
    {
        private readonly DateTime _minSqlDateTime = DateTime.Parse("1/1/1753 12:00:00 AM");
        private readonly IList<PropertyInfo> _properties;
        private readonly IList<PropertyInfo> _manyToOneProperties;

        public AutoDbMapper()
        {
            _properties = (from property in typeof (TMapTo).GetRuntimeProperties().OrderBy(p => p.Name)
                let ignore = property.GetCustomAttributes(typeof (OneToManyAttribute), true).Any() ||
                             property.GetCustomAttributes(typeof (OneToOneAttribute), true).Any() ||
                             property.GetCustomAttributes(typeof (ManyToOneAttribute), true).Any() ||
                             property.GetCustomAttributes(typeof (ManyToManyAttribute), true).Any()
                where !ignore
                select property).ToList();

            _manyToOneProperties = typeof(TMapTo)
                .GetRuntimeProperties()
                .Where(property => property.GetCustomAttributes(typeof(ManyToOneAttribute), true).Any())
                .ToList();
        }

        public IList<string> GetFieldNames()
        {
            var fieldNames = _properties.Select(prop => prop.Name).ToList();
            fieldNames.AddRange(_manyToOneProperties.Select(prop => prop.Name + "Id"));
            return fieldNames;
        }

        public IDictionary<string, object> BuildDbParametersFrom(TMapTo model)
        {
            var dictionary = new Dictionary<string, object>();

            _properties.OrderBy(p => p.Name).ForEach(property =>
            {
                object value = property.GetValue(model);

                if (property.PropertyType == typeof (DateTime))
                {
                    if ((DateTime) value < _minSqlDateTime)
                    {
                        value = DateTimeHelper.MinSqlValue;
                    }
                }
                else if (property.PropertyType == typeof (Guid))
                {
                    if ((Guid) value == Guid.Empty)
                    {
                        value = null;
                    }
                }

                dictionary.Add(property.Name, value);
            });

            AddManyToOneRecords(model, dictionary);

            return dictionary;
        }

        private void AddManyToOneRecords(TMapTo model, Dictionary<string, object> dictionary)
        {
            _manyToOneProperties.ForEach(propertyInfo =>
            {
                var dbColumnName = propertyInfo.Name + "Id";
                var manyToOneObject = propertyInfo.GetValue(model);
                if (manyToOneObject == null)
                {
                    dictionary.Add(dbColumnName, null);
                    return;
                }

                var manyToOneObjectType = manyToOneObject.GetType();
                var idPropertyInfo = manyToOneObjectType.GetRuntimeProperty("Id");
                var idValue = idPropertyInfo.GetValue(manyToOneObject, null);

                if (idPropertyInfo.PropertyType == typeof (Guid))
                {
                    if ((Guid) idValue == Guid.Empty)
                    {
                        idValue = null;
                    }
                }
                else if (idPropertyInfo.PropertyType == typeof (int))
                {
                    if ((int) idValue == 0)
                    {
                        idValue = null;
                    }
                }
                else if (idPropertyInfo.PropertyType == typeof (long))
                {
                    if ((long) idValue == 0)
                    {
                        idValue = null;
                    }
                }

                dictionary.Add(dbColumnName, idValue);
            });
        }

        public IList<TMapTo> BuildListFrom(IDbReader reader)
        {
            var list = new List<TMapTo>();
            TMapTo model = BuildFrom(reader);

            while (model != null)
            {
                list.Add(model);
                model = BuildFrom(reader);
            }

            return list;
        }

        public TMapTo BuildFrom(IDbReader reader)
        {
            if (!reader.Read())
            {
                return null;
            }

            var model = new TMapTo();

            _properties.ForEach(property =>
            {
                if (!reader.IsDbNull(property.Name))
                {
                    property.SetValue(model, reader[property.Name], null);
                }
            });

            _manyToOneProperties.ForEach(property =>
            {
                var propertyName = property.Name + "Id";

                if (!reader.IsDbNull(propertyName))
                {
                    var manyToOneObject = Activator.CreateInstance(property.PropertyType);
                    var idProperty = property.PropertyType.GetRuntimeProperty("Id");
                    idProperty.SetValue(manyToOneObject, reader[propertyName], null);
                    
                    property.SetValue(model, manyToOneObject, null);
                }
            });

            return model;
        }

        public IEnumerable<TMapTo> BuildQueueFrom(IDbReader reader)
        {
            var queue = new Queue<TMapTo>();
            TMapTo model = BuildFrom(reader);

            while (model != null)
            {
                queue.Enqueue(model);
                model = BuildFrom(reader);
            }

            return queue;
        }
    }
}