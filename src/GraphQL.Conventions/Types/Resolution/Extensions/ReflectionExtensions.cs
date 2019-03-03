using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GraphQL.Conventions.Types.Descriptors;

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
                return type.GetElementType().GetTypeInfo();
            }
            return type.IsGenericType
                ? type.GenericTypeArguments.First().GetTypeInfo()
                : null;
        }

        public static TypeInfo TypeParameter(this GraphTypeInfo type)
        {
            return type.TypeRepresentation.TypeParameter();
        }

        public static bool IsPrimitiveGraphType(this TypeInfo type)
        {
            return type.IsPrimitive ||
                   type.IsEnum ||
                   type.IsValueType ||
                   type.AsType() == typeof(string);
        }

        public static bool IsEnumerableGraphType(this TypeInfo type)
        {
            return type.IsGenericType(typeof(List<>)) ||
                   type.IsGenericType(typeof(IList<>)) ||
                   type.IsGenericType(typeof(IEnumerable<>)) ||
                   type.IsArray;
        }

        public static bool IsOfEnumerableGraphType(this object obj) =>
            obj?.GetType().GetTypeInfo().IsEnumerableGraphType() ?? false;

        public static async Task<object> GetTaskResult(this object obj)
        {
            var task = obj as Task;
            await task.ConfigureAwait(false);
            var propertyInfo = task.GetType().GetTypeInfo().GetProperty("Result");
            return propertyInfo.GetValue(task);
        }

        public static TypeInfo GetTypeRepresentation(this TypeInfo typeInfo)
        {
            if (typeInfo.IsGenericType(typeof(Task<>)))
            {
                typeInfo = typeInfo.TypeParameter();
            }
            if (typeInfo.IsGenericType(typeof(Nullable<>)) ||
                typeInfo.IsGenericType(typeof(NonNull<>)))
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
            var genericMethod = convertMethod.MakeGenericMethod(elementType);
            return genericMethod.Invoke(null, new object[] {list});
        }

        public static bool IsExtensionMethod(this MethodInfo methodInfo)
        {
            return methodInfo?.IsDefined(typeof(ExtensionAttribute), false) ?? false;
        }
    }
}
