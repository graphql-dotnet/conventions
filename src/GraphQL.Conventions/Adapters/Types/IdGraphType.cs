using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Language.AST;
using static GraphQL.Conventions.Utilities;

namespace GraphQL.Conventions.Adapters.Types
{
    public class IdGraphType : GraphQL.Types.IdGraphType
    {
        public override object Serialize(object value)
        {
            return value?.ToString();
        }

        public override object ParseValue(object value)
        {
            var id = value?.ToString().StripQuotes();
            return NullableId(id);
        }

        public override object ParseLiteral(IValue value)
        {
            var str = value as StringValue;
            if (str != null)
            {
                return ParseValue(str.Value);
            }
            return NullableId(null);
        }
    }
}
