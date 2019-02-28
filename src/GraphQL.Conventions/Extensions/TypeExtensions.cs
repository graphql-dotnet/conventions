using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GraphQL.Conventions.Extensions
{
    public static class TypeExtension
    {
        /// <summary>
        /// Returns Extensions methods for a given type scoped to the extension typeinfo
        /// </summary>
        /// <remarks>
        /// Inspired by Jon Skeet from his answer on http://stackoverflow.com/questions/299515/c-sharp-reflection-to-identify-extension-methods
        /// </remarks>
        /// <returns>returns MethodInfo[] with the extended Method</returns>

        public static MethodInfo[] GetExtensionMethods(this TypeInfo typeInfo, TypeInfo extensionTypeInfo)
        {
            return extensionTypeInfo != null && typeInfo != null ? 
                extensionTypeInfo
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.IsDefined(typeof(ExtensionAttribute), false) && m.GetParameters()[0].ParameterType == typeInfo.UnderlyingSystemType)
                    .ToArray() : new MethodInfo[] { };
        }
    }
}