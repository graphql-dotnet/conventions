using System.Collections;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public class CollectionUnwrapper : UnwrapperBase
    {
        public CollectionUnwrapper(IUnwrapper parent)
            : base(parent)
        {
        }

        public override object UnwrapValue(object value)
        {
            if (!value.IsOfEnumerableGraphType())
            {
                return value;
            }

            var enumerable = value as IEnumerable;
            return enumerable.Map(element => _parent.Unwrap(element));
        }
    }
}
