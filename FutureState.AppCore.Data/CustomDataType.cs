using System;

namespace FutureState.AppCore.Data
{
    public abstract class CustomDataType
    {
        protected CustomDataType(Type customType, String dialectValue)
        {
            if (customType != null && dialectValue != null)
                Column.AddCustomType(customType, dialectValue);
        }
    }
}