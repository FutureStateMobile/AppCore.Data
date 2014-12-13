using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                    _properties = (from property in typeof(TMapTo).GetRuntimeProperties().OrderBy(p => p.Name) select property).ToList();                    
                }
                return _properties;
            }
        }

        public IList<TMapTo> BuildListFrom(IList<TMapFrom> inputList)
        {
            return inputList.Select(BuildFrom).ToList();
        }

        public TMapTo BuildFrom(TMapFrom input)
        {
            if (input.IsNull())
            {
                return null;
            }

            var model = new TMapTo();

            Properties.ForEach(property =>
            {
                var inputProperty = typeof (TMapFrom).GetRuntimeProperty(property.Name);
                if (inputProperty != null)
                {
                    property.SetValue(model, inputProperty.GetValue(input, null), null);
                }
            });

            return model;
        }
    }
}