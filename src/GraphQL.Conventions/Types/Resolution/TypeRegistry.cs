using System;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public static class TypeRegistry
    {
        private static readonly Dictionary<Type, List<Type>> Registry = new Dictionary<Type, List<Type>>();

        public static void Add<TInterface, TImplementation>()
        {
            Add<TInterface>(typeof(TImplementation));
        }

        public static void Add<TInterface>(params Type[] implementationTypes)
        {
            List<Type> typeList;
            if (!Registry.TryGetValue(typeof(TInterface), out typeList))
            {
                typeList = Registry[typeof(TInterface)] = new List<Type>();
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
            if (Registry.TryGetValue(@type.AsType(), out typeList))
            {
                foreach (var registryType in typeList)
                {
                    yield return registryType;
                }
            }
        }
    }
}
