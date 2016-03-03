using GraphQL.Conventions.Execution;

namespace GraphQL.Conventions.Profiling
{
    public interface IProfiler
    {
        void EnterResolver(ExecutionContext context, long correlationId);

        void ExitResolver(ExecutionContext context, long correlationId);
    }
}
