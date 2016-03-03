using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Types.Resolution
{
    public delegate object ResolutionDelegate(GraphFieldInfo fieldInfo, IResolutionContext context); 
}