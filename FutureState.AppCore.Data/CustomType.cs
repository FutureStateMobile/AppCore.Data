using System;

namespace FutureState.AppCore.Data
{
    public class CustomType
    {
        public CustomType(Type customType, String dialectValue)
        {
            if (customType != null && dialectValue != null)
                Column.AddCustomType(customType, dialectValue);
        }
    }
}