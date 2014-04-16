using System;
using System.Collections.Generic;
using Cirrious.CrossCore.IoC;

namespace FutureState.AppCore.Data
{
    public class MvxServiceLocator : IServiceLocator
    {
        private readonly IMvxIoCProvider _ioCProvider;

        public MvxServiceLocator(IMvxIoCProvider ioCProvider)
        {
            _ioCProvider = ioCProvider;
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<object> GetAllInstances<TService>()
        {
            throw new NotSupportedException();
        }

        public object GetInstance(Type serviceType, string key)
        {
            throw new NotSupportedException();
        }

        public object GetInstance(Type serviceType)
        {
            return _ioCProvider.Resolve(serviceType);
        }

        public TService GetInstance<TService>(string key)
        {
            throw new NotSupportedException();
        }

        public TService GetInstance<TService>()
        {
            return (TService) _ioCProvider.Resolve(typeof (TService));
        }
    }
}