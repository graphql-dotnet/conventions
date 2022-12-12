namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public class NonNullUnwrapper : UnwrapperBase
    {
        public override object UnwrapValue(object value)
        {
            return (value is INonNull result)
                ? result.ObjectValue
                : value;
        }
    }
}
