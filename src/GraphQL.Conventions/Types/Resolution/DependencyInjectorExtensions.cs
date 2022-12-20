using System;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    internal static class DependencyInjectorExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider) => (T)serviceProvider?.GetService(typeof(T));

        public static object GetRequiredService(this IServiceProvider serviceProvider, Type serviceType)
            => serviceProvider.GetService(serviceType)
            ?? throw new InvalidOperationException($"No service for type '{serviceType}' has been registered.");

        public static T GetRequiredService<T>(this IServiceProvider serviceProvider)
            => (T)serviceProvider.GetRequiredService(typeof(T));
    }
}
