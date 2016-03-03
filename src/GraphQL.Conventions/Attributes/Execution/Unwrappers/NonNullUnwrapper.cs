using GraphQL.Conventions.Types;

namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public class NonNullUnwrapper : UnwrapperBase
    {
        public override object UnwrapValue(object value)
        {
            var result = value as INonNull;
            return (result != null)
                ? result.ObjectValue
                : value;
        }
    }
}