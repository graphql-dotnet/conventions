using System.Reflection;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public interface IDependencyInjector
    {
        object Resolve(TypeInfo typeInfo);
    }
}
