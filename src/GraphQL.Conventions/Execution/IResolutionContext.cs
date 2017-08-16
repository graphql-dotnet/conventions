using System.Collections.Generic;
using System.Threading;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Types;

namespace GraphQL.Conventions
{
    public interface IResolutionContext
    {
        object Source { get; }

        object GetArgument(string name, object defaultValue = null);

        object GetArgument(GraphArgumentInfo argument);

        void SetArgument(string name, object value);

        object RootValue { get; }

        IUserContext UserContext { get; }

        IDependencyInjector DependencyInjector { get; }

        GraphFieldInfo FieldInfo { get; }

        CancellationToken CancellationToken { get; }

        IEnumerable<string> Path { get; }

        ResolveFieldContext FieldContext { get; }
    }
}
