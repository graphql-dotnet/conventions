using System.Collections.Generic;
using GraphQL.Execution;

namespace GraphQL.Conventions.Execution
{
    public static class UserContextExtensions
    {
        public static IUserContext GetUserContext(this IProvideUserContext context) => GetValue<IUserContext>(context.UserContext);
        public static void SetUserContext(this IProvideUserContext context, IUserContext value) => SetValue(context.UserContext, value);

        public static IDependencyInjector GetDependencyInjector(this IProvideUserContext context) => GetValue<IDependencyInjector>(context.UserContext);
        public static void SetDependencyInjector(this IProvideUserContext context, IDependencyInjector value) => SetValue(context.UserContext, value);

        private static T GetValue<T>(IDictionary<string, object> dictionary)
        {
            var key = typeof(T).FullName;
            return dictionary != null && dictionary.TryGetValue(key, out var value) 
                ? (T)value
                : default;
        }

        private static void SetValue<T>(IDictionary<string, object> dictionary, T value)
        {
            var key = typeof(T).FullName;
            if (dictionary != null)
            {
                if (dictionary.ContainsKey(key))
                    dictionary[key] = value;
                else
                    dictionary.Add(key, value);
            }
        }
    }
}