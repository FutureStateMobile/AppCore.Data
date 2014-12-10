using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FutureState.AppCore.Data.Extensions;

namespace FutureState.AppCore.Data
{
    public class AutoModelMapper<TMapTo> : IModelMapper<TMapTo> where TMapTo : class, new()
    {
        public AutoModelMapper()
        {
            _properties = ( from property in typeof( TMapTo ).GetRuntimeProperties().OrderBy( p => p.Name ) select property ).ToList();
//                let ignore = property.GetCustomAttributes( typeof( OneToManyAttribute ), true ).Any() ||
//                             property.GetCustomAttributes( typeof( OneToOneAttribute ), true ).Any() ||
//                             property.GetCustomAttributes( typeof( ManyToManyAttribute ), true ).Any()
//                where !ignore
//                select property ).ToList();
        }

        private readonly IList<PropertyInfo> _properties;

        public IList<TMapTo> BuildListFrom<TInput> ( IList<TInput> inputList ) where TInput : class
        {
            return inputList.Select( BuildFrom ).ToList();
        }

        public TMapTo BuildFrom<TInput> ( TInput input ) where TInput : class
        {
            if ( input.IsNull() )
            {
                return null;
            }

            var model = new TMapTo();

            _properties.ForEach( property =>
            {
                var inputProperty = typeof( TInput ).GetRuntimeProperty( property.Name );
                if ( inputProperty != null )
                {
                    property.SetValue( model, inputProperty.GetValue( input, null ), null );
                }
            } );

            return model;
        }
    }
}