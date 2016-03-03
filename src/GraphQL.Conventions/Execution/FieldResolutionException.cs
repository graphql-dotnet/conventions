using System;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Execution
{
    public class FieldResolutionException : Exception
    {
        public FieldResolutionException(GraphFieldInfo fieldInfo, IResolutionContext context, Exception exception)
            : base($"Unable to resolve field '{fieldInfo.Name}' on type '{fieldInfo.DeclaringType.Name}': {exception.Message}", exception)
        {
            FieldInfo = fieldInfo;
            ResolutionContext = context;
        }

        public GraphFieldInfo FieldInfo { get; private set; }

        public IResolutionContext ResolutionContext { get; private set; }
    }
}
