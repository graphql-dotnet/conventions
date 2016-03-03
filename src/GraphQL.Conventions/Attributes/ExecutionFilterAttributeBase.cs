using GraphQL.Conventions.Execution;

namespace GraphQL.Conventions.Attributes
{
    public abstract class ExecutionFilterAttributeBase : AttributeBase, IExecutionFilterAttribute
    {
        protected ExecutionFilterAttributeBase()
            : base(AttributeApplicationPhase.ExecutionFilter)
        {
        }

        protected ExecutionFilterAttributeBase(AttributeApplicationPhase phase)
            : base(phase)
        {
        }

        public virtual bool IsEnabled(ExecutionContext context)
        {
            return true;
        }

        public virtual void AfterExecution(ExecutionContext context, long correlationId)
        {
        }

        public virtual void BeforeExecution(ExecutionContext context, long correlationId)
        {
        }

        public virtual bool ShouldExecute(ExecutionContext context)
        {
            return true;
        }
    }
}
