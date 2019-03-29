namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public class OptionalUnwrapper : UnwrapperBase
    {
        public override object UnwrapValue(object value)
        {
            var result = value as IOptional;
            return (result != null)
                ? result.ObjectValue
                : value;
        }
    }
}