using System.Threading.Tasks;
using GraphQL.Conventions.Attributes.Execution.Unwrappers;
using GraphQL.Conventions.Execution;

namespace GraphQL.Conventions.Attributes
{
    public abstract class ExecutionFilterAttributeBase : AttributeBase, IExecutionFilterAttribute
    {
        private readonly IUnwrapper _unwrapper = new ValueUnwrapper();

        protected ExecutionFilterAttributeBase()
            : base(AttributeApplicationPhase.ExecutionFilter)
        {
        }

        protected ExecutionFilterAttributeBase(AttributeApplicationPhase phase)
            : base(phase)
        {
        }

        public virtual Task<object> Execute(IResolutionContext context, FieldResolutionDelegate next)
        {
            return next(context);
        }

        protected object Unwrap(object value) =>
            _unwrapper.Unwrap(value);
    }
}
