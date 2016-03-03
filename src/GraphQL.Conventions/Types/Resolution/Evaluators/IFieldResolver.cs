using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Types.Resolution.Evaluators
{
    public interface IFieldResolver
    {
        object GetValue(GraphFieldInfo fieldInfo, IResolutionContext context);

        object CallMethod(GraphFieldInfo fieldInfo, IResolutionContext context);
    }
}
