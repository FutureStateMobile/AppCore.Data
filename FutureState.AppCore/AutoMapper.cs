using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FutureState.AppCore.Data.Attributes;
using FutureState.AppCore.Data.Extensions;
using FutureState.AppCore.Data.Helpers;

namespace FutureState.AppCore.Data
{
    public class AutoMapper<TMapTo> : IAutoMapper<TMapTo> where TMapTo : class, new()
    {
        private readonly DateTime _minSqlDateTime = DateTime.Parse("1/1/1753 12:00:00 AM");

        public IDictionary<string, object> BuildDbParametersFrom(TMapTo model)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (PropertyInfo property in model.GetType().GetProperties().OrderBy(p => p.Name))
            {
                object value = property.GetValue(model, null);

                if (property.PropertyType == typeof (DateTime))
                {
                    if ((DateTime) value < _minSqlDateTime)
                    {
                        value = DateTimeHelper.MinSqlValue;
                    }
                }

                // Check if the model object is to be ignored.
                bool ignore = property.GetCustomAttributes(typeof (OneToManyAttribute), true).Any() ||
                              property.GetCustomAttributes(typeof (ManyToManyAttribute), true).Any();

                if (!ignore)
                {
                    dictionary.Add(property.Name, value);
                }
            }

            return dictionary;
        }

        public IList<string> GetFieldNameList(TMapTo model)
        {
            return (from property in model.GetType().GetProperties().OrderBy(p => p.Name)
                    let ignore = property.GetCustomAttributes(typeof (OneToManyAttribute), true).Any() ||
                                 property.GetCustomAttributes(typeof (ManyToManyAttribute), true).Any()
                    where !ignore
                    select property.Name).ToList();
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

            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                // hack: a try/catch to handle DBNull to String converstion.
                try
                {
                    property.SetValue(model, reader[property.Name], null);
                }
                catch (ArgumentException)
                {
                    property.SetValue(model, null, null);
                }
            }

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

            foreach (PropertyInfo prop in model.GetType().GetProperties())
            {
                PropertyInfo getProp = input.GetType().GetProperty(prop.Name);
                if (getProp != null)
                {
                    prop.SetValue(model, getProp.GetValue(input, null), null);
                }
            }

            return model;
        }

        public IEnumerable<TMapTo> BuildStackFrom(IDbReader reader)
        {
            var stack = new Stack<TMapTo>();

            TMapTo model = BuildFrom(reader);

            while (model != null)
            {
                stack.Push(model);
                model = BuildFrom(reader);
            }

            return stack;
        }
    }
}