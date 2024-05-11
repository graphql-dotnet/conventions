using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GraphQL.Conventions.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.DataLoader;

namespace GraphQL.Conventions.Types.Resolution.Extensions
{
    public static class ReflectionExtensions
    {
        public static TType CreateInstance<TType>(this Type type, TypeInfo typeArg, params object[] args)
            where TType : class
        {
            var genericType = type.MakeGenericType(typeArg.AsType());
            return Activator.CreateInstance(genericType, args) as TType;
        }

        public static bool IsGenericType(this TypeInfo type, Type genericType)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }

        public static bool IsNullableType(this TypeInfo type)
        {
            return type.IsGenericType(typeof(Nullable<>));
        }

        public static Type GetImplementInterface(this Type type, Type interfaceType, bool fuseGeneric = true)
        {
            if (!interfaceType.IsInterface) return null;
            fuseGeneric &= interfaceType.IsGenericType;
            if (type.IsGenericType && fuseGeneric
                    ? type.GetGenericTypeDefinition() == interfaceType.GetGenericTypeDefinition()
                    : type == interfaceType
               ) return interfaceType;
            while (type is not null)
            {
                var interfaces = type.GetInterfaces();
                var mayFusedGenericInterface = fuseGeneric
                    ? interfaces.Select(t => t.IsGenericType ? t.GetGenericTypeDefinition() : t).ToArray()
                    : interfaces;

                for (int i = 0; i < interfaces.Length; i++)
                {
                    var @interface = mayFusedGenericInterface[i];
                    if (@interface == interfaceType) return interfaces[i];
                    var ni = @interface.GetImplementInterface(interfaceType, fuseGeneric);
                    if (ni is not null) return ni;
                }

                type = type.BaseType;
            }

            return null;
        }

        public static bool ImplementInterface(this Type type, Type interfaceType, bool fuseGeneric = true) =>
            type.GetImplementInterface(interfaceType, fuseGeneric) is not null;


        public static TypeInfo BaseType(this TypeInfo type)
        {
            return type.IsNullableType()
                ? type.TypeParameter()
                : type;
        }

        public static TypeInfo TypeParameter(this TypeInfo type)
        {
            if (type.IsArray)
            {
                return type.GetElementType()?.GetTypeInfo();
            }
            return type.IsGenericType
                ? type.GenericTypeArguments.First().GetTypeInfo()
                : type.TypeParameterCollection();
        }

        public static TypeInfo TypeParameter(this GraphTypeInfo type)
        {
            return type.TypeRepresentation.TypeParameter();
        }

        public static TypeInfo TypeParameterCollection(this TypeInfo type) => (
            type.GetImplementInterface(typeof(ICollection<>)) ??
            type.GetImplementInterface(typeof(IReadOnlyList<>))
        )?.GetTypeInfo();

        public static bool IsPrimitiveGraphType(this TypeInfo type)
        {
            return type.IsPrimitive ||
                   type.IsEnum ||
                   type.IsValueType ||
                   type.AsType() == typeof(string);
        }

        public static bool IsEnumerableGraphType(this TypeInfo type)
        {
            return type.ImplementInterface(typeof(ICollection<>)) ||
                   type.ImplementInterface(typeof(IReadOnlyCollection<>), true) ||
                   type.IsGenericType(typeof(IEnumerable<>)) ||
                   (type.IsGenericType && type.DeclaringType == typeof(Enumerable)) || // Handles internal Iterator implementations for LINQ; for reference https://referencesource.microsoft.com/#system.core/System/Linq/Enumerable.cs
                   type.IsArray;
        }

        public static bool IsOfEnumerableGraphType(this object obj) =>
            obj?.GetType().GetTypeInfo().IsEnumerableGraphType() ?? false;

        public static async Task<object> GetTaskResult(this object obj)
        {
            if (!(obj is Task task))
                return null;
            await task.ConfigureAwait(false);
            var propertyInfo = task.GetType().GetTypeInfo().GetProperty("Result");
            return propertyInfo?.GetValue(task);
        }

        public static TypeInfo GetTypeRepresentation(this TypeInfo typeInfo)
        {
            while (typeInfo.IsGenericType(typeof(IDataLoaderResult<>)))
            {
                typeInfo = typeInfo.TypeParameter();
            }

            if (typeInfo.IsGenericType(typeof(Task<>)))
            {
                typeInfo = typeInfo.TypeParameter();
            }

            if (typeInfo.IsGenericType(typeof(IObservable<>)))
            {
                typeInfo = typeInfo.TypeParameter();
            }

            if (typeInfo.IsGenericType(typeof(Nullable<>)) ||
                typeInfo.IsGenericType(typeof(NonNull<>)) ||
                typeInfo.IsGenericType(typeof(Optional<>)))
            {
                typeInfo = typeInfo.TypeParameter();
            }

            return typeInfo;
        }

        public static TypeInfo GetTypeRepresentation(this Type typeInfo) =>
            typeInfo.GetTypeInfo().GetTypeRepresentation();

        public static TypeInfo GetTypeRepresentation(this GraphTypeInfo typeInfo) =>
            typeInfo.TypeRepresentation.GetTypeRepresentation();

        public static T[] ConvertToArray<T>(IList list)
        {
            return list.Cast<T>().ToArray();
        }

        public static object ConvertToArrayRuntime(this IList list, Type elementType)
        {
            var convertMethod = typeof(ReflectionExtensions)
                .GetTypeInfo()
                .GetMethod("ConvertToArray", BindingFlags.Static | BindingFlags.Public);
            if (convertMethod == null)
                return null;
            var genericMethod = convertMethod.MakeGenericMethod(elementType);
            return genericMethod.InvokeEnhanced(null, new object[] { list });
        }

        public static bool IsExtensionMethod(this MethodInfo methodInfo)
        {
            return methodInfo?.IsDefined(typeof(ExtensionAttribute), false) ?? false;
        }
    }
}
