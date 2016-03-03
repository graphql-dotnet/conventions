using System.Threading;

namespace GraphQL.Conventions.Execution
{
    public interface IResolutionContext
    {
        object Source { get; }

        object GetArgument(string name, object defaultValue = null);

        void SetArgument(string name, object value);

        object RootValue { get; }

        object UserContext { get; }

        CancellationToken CancellationToken { get; }
    }
}
