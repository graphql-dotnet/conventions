using System;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Execution;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions.Relay
{
    [AttributeUsage(Fields, AllowMultiple = true, Inherited = true)]
    public class RelayMutationAttribute : ExecutionFilterAttributeBase
    {
        public override async Task<object> Execute(IResolutionContext context, FieldResolutionDelegate next)
        {
            var output = await next(context).ConfigureAwait(false);
            var input = Unwrap(context.GetArgument("input"));

            if (input is IRelayMutationInputObject inputObj && output is IRelayMutationOutputObject outputObj)
            {
                outputObj.ClientMutationId = inputObj.ClientMutationId;
            }

            return output;
        }
    }
}
