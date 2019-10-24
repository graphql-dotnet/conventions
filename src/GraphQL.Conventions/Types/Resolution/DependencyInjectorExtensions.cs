using System.Reflection;

namespace GraphQL.Conventions
{
    public static class DependencyInjectorExtensions
    {
        public static T Resolve<T>(this IDependencyInjector dependencyInjector) => (T)dependencyInjector?.Resolve(typeof(T).GetTypeInfo());
    }
}