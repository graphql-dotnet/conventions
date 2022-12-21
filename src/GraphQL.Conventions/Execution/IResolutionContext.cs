using System;
using System.Collections.Generic;
using System.Threading;
using GraphQL.Conventions.Types.Descriptors;

// ReSharper disable once CheckNamespace
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

        IServiceProvider ServiceProvider { get; }

        GraphFieldInfo FieldInfo { get; }

        CancellationToken CancellationToken { get; }

        IEnumerable<string> Path { get; }

        IResolveFieldContext FieldContext { get; }
    }
}
