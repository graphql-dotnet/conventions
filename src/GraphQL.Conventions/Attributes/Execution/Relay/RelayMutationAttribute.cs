using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Descriptors.Extensions;
using GraphQL.Conventions.Types.Relay;

namespace GraphQL.Conventions.Attributes.Execution.Relay
{
    public class RelayMutationAttribute : ExecutionFilterAttributeBase
    {
        public override bool IsEnabled(ExecutionContext context)
        {
            var input = context?.ResolutionContext?.GetArgument("input");
            if (input is INonNull)
            {
                input = ((INonNull)input).ObjectValue;
            }
            return input is IRelayMutationInputObject &&
                   context.Entity.Type.HasField("clientMutationId");
        }

        public override void AfterExecution(ExecutionContext context, long correlationId)
        {
            var inputRaw = context.ResolutionContext.GetArgument("input");
            if (inputRaw is INonNull)
            {
                inputRaw = ((INonNull)inputRaw).ObjectValue;
            }

            var outputRaw = context.Result;
            if (outputRaw is INonNull)
            {
                outputRaw = ((INonNull)outputRaw).ObjectValue;
            }

            var input = inputRaw as IRelayMutationInputObject;
            var output = outputRaw as IRelayMutationOutputObject;
            if (input != null && output != null)
            {
                output.ClientMutationId = input.ClientMutationId;
            }
        }
    }
}