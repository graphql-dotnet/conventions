using System;
using System.Collections.Generic;

namespace GraphQL.Conventions.Tests.Server
{
    public class DependencyInjector : IServiceProvider
    {
        private readonly Dictionary<Type, object> _registrations =
            new Dictionary<Type, object>();

        public void Register<TType>(TType instance)
        {
            _registrations.Add(typeof(TType), instance);
        }

        public object GetService(Type serviceType)
        {
            if (_registrations.TryGetValue(serviceType, out object instance))
            {
                return instance;
            }
            return null;
        }
    }
}
