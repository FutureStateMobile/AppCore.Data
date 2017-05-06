using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FutureState.AppCore.Data.Attributes;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data
{
    public class AutoDbMapper<TModel> : IDbMapper<TModel> where TModel : class, new()
    {
        private IList<PropertyInfo> _properties;
        private IList<PropertyInfo> _manyToOneProperties;
        private List<string> _fieldNames;

        private IEnumerable<PropertyInfo> ManyToOneProperties
        {
            get
            {
                if (_manyToOneProperties == null)
                {
                    _manyToOneProperties = typeof(TModel)
                        .GetRuntimeProperties()
                        .Where(property => property.GetCustomAttributes(typeof(ManyToOneAttribute), true).Any())
                        .ToList();
                }

                return _manyToOneProperties;
            }
        }

        private IEnumerable<PropertyInfo> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = (from property in typeof(TModel).GetRuntimeProperties().OrderBy(p => p.Name)
                                   let ignore = property.GetCustomAttributes(typeof(OneToManyAttribute), true).Any() ||
                                                property.GetCustomAttributes(typeof(OneToOneAttribute), true).Any() ||
                                                property.GetCustomAttributes(typeof(ManyToOneAttribute), true).Any() ||
                                                property.GetCustomAttributes(typeof(ManyToManyAttribute), true).Any() ||
                                                property.GetCustomAttributes(typeof(IgnoreAttribute), true).Any()
                                   where !ignore
                                   select property).ToList();
                }
                return _properties;
            }
        }

        public virtual IList<string> FieldNames
        {
            get
            {
                if (_fieldNames == null)
                {
                    _fieldNames = Properties.Select(prop => prop.Name).ToList();
                    _fieldNames.AddRange(ManyToOneProperties.Select(prop => prop.Name + "Id"));
                }
                return _fieldNames;
            }
        }

        public virtual IDictionary<string, object> BuildDbParametersFrom(TModel model)
        {
            var dictionary = new Dictionary<string, object>();

            Properties.OrderBy(p => p.Name).ForEach(property =>
            {
                var value = property.GetValue(model);

                if (property.PropertyType == typeof (DateTime))
                {
                    value = ((DateTime) value).GetDbSafeDate();
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

        private void AddManyToOneRecords(TModel model, IDictionary<string, object> dictionary)
        {
            ManyToOneProperties.ForEach(propertyInfo =>
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

        public virtual IEnumerable<TModel> BuildListFrom(IDbReader reader)
        {
            var list = new List<TModel>();
            var model = BuildFrom(reader);

            while (model != null)
            {
                list.Add(model);
                model = BuildFrom(reader);
            }

            return list;
        }

        public virtual TModel BuildFrom(IDbReader reader)
        {
            if (!reader.Read())
            {
                return null;
            }

            var model = new TModel();

            Properties.ForEach(property =>
            {
                if (!reader.IsDbNull(property.Name))
                {
                    property.SetValue(model, reader[property.Name], null);
                }
            });

            ManyToOneProperties.ForEach(property =>
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

        public virtual IEnumerable<TModel> BuildQueueFrom(IDbReader reader)
        {
            var queue = new Queue<TModel>();
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