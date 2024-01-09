namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public class UnionUnwrapper : UnwrapperBase
    {
        public override object UnwrapValue(object value)
        {
            return (value is Union result)
                ? result.Instance
                : value;
        }
    }
}
