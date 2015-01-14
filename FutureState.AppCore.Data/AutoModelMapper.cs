using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FutureState.AppCore.Data.Attributes;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data
{
    public class AutoModelMapper<TMapTo, TMapFrom> : IModelMapper<TMapTo, TMapFrom>
        where TMapTo : class, new()
        where TMapFrom : class, new()
    {
        private IList<PropertyInfo> _properties;
        internal IList<PropertyInfo> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = (from property in typeof(TMapTo).GetRuntimeProperties().OrderBy(p => p.Name)
                                   let ignore = property.GetCustomAttributes(typeof(IgnoreAttribute), true).Any()
                                   where !ignore
                                   where property.CanWrite
                                   select property).ToList();
                }
                return _properties;
            }
        }

        public virtual IEnumerable<TMapTo> BuildListFrom(IEnumerable<TMapFrom> inputList)
        {
            return inputList.Select(BuildFrom);
        }

        public virtual TMapTo BuildFrom(TMapFrom input, TMapTo output)
        {
            if (input.IsNull())
            {
                return null;
            }

            if (output.IsNull())
            {
                output = new TMapTo();
            }

            Properties.ForEach(property =>
            {
                var inputProperty = typeof (TMapFrom).GetRuntimeProperty(property.Name);
                if (inputProperty != null)
                {
                    property.SetValue(output, inputProperty.GetValue(input, null), null);
                }
            });
            
            return output;
        }

        public virtual TMapTo BuildFrom(TMapFrom input)
        {
            if (input.IsNull())
            {
                return null;
            }

            var model = new TMapTo();

            return BuildFrom(input, model);
        }
    }
}