using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Language.AST;
using GraphQL.Types;
using static GraphQL.Conventions.Utilities;

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
            return NullableCursor(cursor);
        }

        public override object ParseLiteral(IValue value)
        {
            if (value is StringValue str)
            {
                return ParseValue(str.Value);
            }
            return NullableCursor(null);
        }
    }
}
