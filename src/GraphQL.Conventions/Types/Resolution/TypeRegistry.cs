using System;
using System.Collections.Generic;
using System.Reflection;

namespace GraphQL.Conventions
{
    public static class TypeRegistry
    {
        private static readonly Dictionary<Type, List<Type>> _registry = new Dictionary<Type, List<Type>>();

        public static void Add<TInterface, TImplementation>()
        {
            Add<TInterface>(typeof(TImplementation));
        }

        public static void Add<TInterface>(params Type[] implementationTypes)
        {
            List<Type> typeList;
            if (!_registry.TryGetValue(typeof(TInterface), out typeList))
            {
                typeList = _registry[typeof(TInterface)] = new List<Type>();
            }
            typeList.AddRange(implementationTypes);
        }

        public static IEnumerable<Type> Get(TypeInfo @type)
        {
            foreach (var assemblyType in @type.Assembly.GetTypes())
            {
                yield return assemblyType;
            }

            List<Type> typeList;
            if (_registry.TryGetValue(@type.AsType(), out typeList))
            {
                foreach (var registryType in typeList)
                {
                    yield return registryType;
                }
            }
        }
    }
}
