using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Relay;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace GraphQL.Conventions.Adapters.Types.Relay
{
    public class CursorGraphType : ScalarGraphType
    {
        public CursorGraphType()
        {
            Name = TypeNames.Cursor;
            Description = "The `Cursor` scalar type is an ordered key used in paginated Relay connections.";
        }

        public override object Serialize(object value)
        {
            return value?.ToString();
        }

        public override object ParseValue(object value)
        {
            var cursor = value?.ToString().StripQuotes();
            return string.IsNullOrWhiteSpace(cursor)
                ? null
                : (Cursor?)cursor;
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
