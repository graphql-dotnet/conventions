using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types;
using GraphQL.Language.AST;

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
            return string.IsNullOrWhiteSpace(id)
                ? null
                : (Id?)id;
        }

        public override object ParseLiteral(IValue value)
        {
            var str = value as StringValue;
            if (str != null)
            {
                return ParseValue(str.Value);
            }
            return null;
        }
    }
}
