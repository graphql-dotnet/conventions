using System.Collections;
using System.Linq;
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

            //map was removed from GraphQL in https://github.com/graphql-dotnet/graphql-dotnet/commit/952b4ef6950e51bbfe009354f0520180c343e8d4#diff-992f25705b68d8de6f63113661c3a037
            var enumerable = value as IEnumerable;
            return from object item in enumerable
                   select _parent.Unwrap(item);
        }
    }
}
