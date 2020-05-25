namespace GraphQL.Conventions
{
    public interface IDependencyInjectorAccessor
    {
        IDependencyInjector DependencyInjector { get; }
    }
}