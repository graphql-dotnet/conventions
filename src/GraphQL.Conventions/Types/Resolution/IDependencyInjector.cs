using System.Reflection;

namespace GraphQL.Conventions
{
    public interface IDependencyInjector
    {
        object Resolve(TypeInfo typeInfo);
    }
}
