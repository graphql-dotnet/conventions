using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQL.Conventions.Types.Resolution
{
    public interface ICanChangeReflectorSettings
    { }

    public static class ReflectorSettingsExtensions
    {
        static object _lockKey = new object();

        public static T IgnoreTypesFromNamespacesStartingWith<T>(this T source, params string[] namespacesToIgnore) where T : ICanChangeReflectorSettings
        {
            if (namespacesToIgnore == null || !namespacesToIgnore.Any())
                return source;

            namespacesToIgnore = namespacesToIgnore.Distinct().ToArray();
            foreach (var @namespace in namespacesToIgnore)
                if (!ObjectReflectorSettings.IgnoredNamespaces.Contains(@namespace))
                {
                    lock (_lockKey)
                        ObjectReflectorSettings.IgnoredNamespaces.Add(@namespace);
                }

            return source;
        }

        public static T IgnoreFieldsWithVoidReturnType<T>(this T source, bool ignoreFields = true) where T : ICanChangeReflectorSettings
        {
            ObjectReflectorSettings.IgnoreFieldsWithVoidReturnType = ignoreFields;
            return source;
        }


        public static T ResetIgnoredNamespaces<T>(this T source)
        {
            ResetIgnoredNamespaces();
            return source;
        }

        public static void ResetIgnoredNamespaces()
        {
            lock (_lockKey)
            {
                ObjectReflectorSettings.IgnoredNamespaces.Clear();
                ObjectReflectorSettings.IgnoredNamespaces.Add("System.");
            }
        }

        public static void ResetIgnoreFieldWithVoidReturnType()
        {
            lock (_lockKey)
            {
                ObjectReflectorSettings.IgnoreFieldsWithVoidReturnType = false;
            }
        }

        public static void Reset()
        {
            ResetIgnoredNamespaces();
            ResetIgnoreFieldWithVoidReturnType();
        }
    }

}
