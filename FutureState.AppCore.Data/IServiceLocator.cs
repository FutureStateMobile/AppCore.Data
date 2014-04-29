using System;
using System.Collections.Generic;

namespace FutureState.AppCore.Data
{
    public interface IServiceLocator
    {
        IEnumerable<object> GetAllInstances(Type serviceType);
        IEnumerable<TService> GetAllInstances<TService>();
        object GetInstance(Type serviceType, string key);
        object GetInstance(Type serviceType);
        TService GetInstance<TService>(string key);
        TService GetInstance<TService>();
    }
}
