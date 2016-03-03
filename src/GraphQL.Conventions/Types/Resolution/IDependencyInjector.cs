using System.Reflection;

namespace GraphQL.Conventions.Types.Resolution
{
    public interface IDependencyInjector
    {
        object Resolve(TypeInfo typeInfo);
    }
}
