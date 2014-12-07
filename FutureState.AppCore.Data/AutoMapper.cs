using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FutureState.AppCore.Data.Attributes;
using FutureState.AppCore.Data.Extensions;
using FutureState.AppCore.Data.Helpers;

namespace FutureState.AppCore.Data
{
    public class AutoMapper<TMapTo> : IModelMapper<TMapTo> where TMapTo : class, new()
    {
        private readonly DateTime _minSqlDateTime = DateTime.Parse("1/1/1753 12:00:00 AM");
        private readonly IList<PropertyInfo> _properties;

        public AutoMapper()
        {
            _properties = (from property in typeof (TMapTo).GetRuntimeProperties().OrderBy(p => p.Name)
                let ignore = property.GetCustomAttributes(typeof (OneToManyAttribute), true).Any() ||
                             property.GetCustomAttributes(typeof (OneToOneAttribute), true).Any() ||
                             property.GetCustomAttributes(typeof (ManyToManyAttribute), true).Any()
                where !ignore
                select property).ToList();
        }

        public IDictionary<string, object> BuildDbParametersFrom(TMapTo model)
        {
            var dictionary = new Dictionary<string, object>();

            _properties.OrderBy(p => p.Name).ForEach(property =>
            {
                var value = property.GetValue(model, null);

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

            return dictionary;
        }

        public IList<string> GetFieldNameList()
        {
            return _properties.Select(p => p.Name).ToList();
        }

        public IList<TMapTo> BuildListFrom(IDbReader reader)
        {
            var list = new List<TMapTo>();
            var model = BuildFrom(reader);

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
                    property.SetValue(model, reader[property.Name]);
                }
            });

            return model;
        }

        public IList<TMapTo> BuildListFrom<TInput>(IList<TInput> inputList) where TInput : class
        {
            return inputList.Select(BuildFrom).ToList();
        }

        public TMapTo BuildFrom<TInput>(TInput input) where TInput : class
        {
            if (input.IsNull())
            {
                return null;
            }

            var model = new TMapTo();

            _properties.ForEach(property =>
            {
                var inputProperty = typeof (TInput).GetRuntimeProperty(property.Name);
                if (inputProperty != null)
                {
                    property.SetValue(model, inputProperty.GetValue(input, null), null);
                }
            });

            return model;
        }

        public IEnumerable<TMapTo> BuildQueueFrom(IDbReader reader)
        {
            var queue = new Queue<TMapTo>();
            var model = BuildFrom(reader);

            while (model != null)
            {
                queue.Enqueue(model);
                model = BuildFrom(reader);
            }

            return queue;
        }
    }
}