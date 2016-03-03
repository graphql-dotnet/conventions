using System.Linq;
using GraphQL.Conventions.Attributes.Collectors;
using GraphQL.Conventions.Execution;

namespace GraphQL.Conventions.Attributes.Execution
{
    public class ProfilableAttribute : ExecutionFilterAttributeBase, IDefaultAttribute
    {
        public override bool IsEnabled(ExecutionContext context)
        {
            return context.Entity.IsProfilable && context.Profilers.Any();
        }

        public override void AfterExecution(ExecutionContext context, long correlationId)
        {
            foreach (var profiler in Enumerable.Reverse(context.Profilers))
            {
                profiler.ExitResolver(context, correlationId);
            }
        }

        public override void BeforeExecution(ExecutionContext context, long correlationId)
        {
            foreach (var profiler in context.Profilers)
            {
                profiler.EnterResolver(context, correlationId);
            }
        }
    }
}