using GraphQL.Conventions.Types;

namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public class UnionUnwrapper : UnwrapperBase
    {
        public override object UnwrapValue(object value)
        {
            var result = value as Union;
            return (result != null)
                ? result.Instance
                : value;
        }
    }
}