using System;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public static class DependencyInjectorExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider) => (T)serviceProvider?.GetService(typeof(T));

        internal static object GetRequiredService(this IServiceProvider serviceProvider, Type serviceType)
            => serviceProvider.GetService(serviceType)
            ?? throw new InvalidOperationException($"No service for type '{serviceType}' has been registered.");

        internal static T GetRequiredService<T>(this IServiceProvider serviceProvider)
            => (T)serviceProvider.GetRequiredService(typeof(T));
    }
}
