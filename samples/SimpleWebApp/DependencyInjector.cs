using System.Collections.Generic;
using System.Reflection;

namespace GraphQL.Conventions.Tests.Server
{
    public class DependencyInjector : IDependencyInjector
    {
        private readonly Dictionary<TypeInfo, object> _registrations =
            new Dictionary<TypeInfo, object>();

        public void Register<TType>(TType instance)
        {
            _registrations.Add(typeof(TType).GetTypeInfo(), instance);
        }

        public object Resolve(TypeInfo typeInfo)
        {
            if (_registrations.TryGetValue(typeInfo, out object instance))
            {
                return instance;
            }
            return null;
        }
    }
}
