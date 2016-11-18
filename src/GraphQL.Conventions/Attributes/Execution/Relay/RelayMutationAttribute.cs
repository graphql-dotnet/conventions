using System.Threading.Tasks;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Relay;

namespace GraphQL.Conventions.Attributes.Execution.Relay
{
    public class RelayMutationAttribute : ExecutionFilterAttributeBase
    {
        public override async Task<object> Execute(IResolutionContext context, FieldResolutionDelegate next)
        {
            var output = await next(context).ConfigureAwait(false);
            var input = Unwrap(context.GetArgument("input"));

            var inputObj = input as IRelayMutationInputObject;
            var outputObj = output as IRelayMutationOutputObject;
            if (inputObj != null && outputObj != null)
            {
                outputObj.ClientMutationId = inputObj.ClientMutationId;
            }

            return output;
        }
    }
}