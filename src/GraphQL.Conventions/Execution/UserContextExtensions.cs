using System.Collections.Generic;
using GraphQL.Execution;
using GraphQL.Types;

namespace GraphQL.Conventions.Execution
{
    public static class UserContextExtensions
    {
        public static IUserContext GetUserContext(this ExecutionContext context) => GetValue<IUserContext>(context.UserContext);
        public static IUserContext GetUserContext(this ResolveFieldContext context) => GetValue<IUserContext>(context.UserContext);
        public static IUserContext GetUserContext<T>(this ResolveFieldContext<T> context) => GetValue<IUserContext>(context.UserContext);

        public static void SetUserContext(this ExecutionContext context, IUserContext value) => SetValue(context.UserContext, value);
        public static void SetUserContext(this ResolveFieldContext context, IUserContext value) => SetValue(context.UserContext, value);
        public static void SetUserContext<T>(this ResolveFieldContext<T> context, IUserContext value) => SetValue(context.UserContext, value);

        public static IDependencyInjector GetDependencyInjector(this ExecutionContext context) => GetValue<IDependencyInjector>(context.UserContext);
        public static IDependencyInjector GetDependencyInjector(this ResolveFieldContext context) => GetValue<IDependencyInjector>(context.UserContext);
        public static IDependencyInjector GetDependencyInjector<T>(this ResolveFieldContext<T> context) => GetValue<IDependencyInjector>(context.UserContext);
        
        public static void SetDependencyInjector(this ExecutionContext context, IDependencyInjector value) => SetValue(context.UserContext, value);
        public static void SetDependencyInjector(this ResolveFieldContext context, IDependencyInjector value) => SetValue(context.UserContext, value);
        public static void SetDependencyInjector<T>(this ResolveFieldContext<T> context, IDependencyInjector value) => SetValue(context.UserContext, value);

        private static T GetValue<T>(IDictionary<string, object> dictionary)
        {
            var key = typeof(T).FullName;
            return dictionary != null && dictionary.ContainsKey(key) 
                ? (T) dictionary[key] 
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