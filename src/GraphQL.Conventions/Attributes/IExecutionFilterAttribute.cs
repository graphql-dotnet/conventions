using GraphQL.Conventions.Execution;

namespace GraphQL.Conventions.Attributes
{
    public interface IExecutionFilterAttribute : IAttribute
    {
        bool IsEnabled(ExecutionContext context);

        void AfterExecution(ExecutionContext context, long correlationId);

        void BeforeExecution(ExecutionContext context, long correlationId);

        bool ShouldExecute(ExecutionContext context);
    }
}