namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public class OptionalUnwrapper : UnwrapperBase
    {
        public override object UnwrapValue(object value)
        {
            return (value is IOptional result)
                ? result.ObjectValue
                : value;
        }
    }
}
