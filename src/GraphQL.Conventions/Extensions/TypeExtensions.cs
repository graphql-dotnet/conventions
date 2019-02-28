using System;
using System.Collections.Generic;
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
        public static MethodInfo[] GetExtensionMethods(this TypeInfo extensionTypeInfo)
        {
            return
                extensionTypeInfo?
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.IsDefined(typeof(ExtensionAttribute), false))
                    .ToArray();
        }
    }
}