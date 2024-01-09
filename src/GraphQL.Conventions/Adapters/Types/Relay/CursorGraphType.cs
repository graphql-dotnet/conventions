using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Types;
using GraphQLParser.AST;
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

        public override object ParseLiteral(GraphQLValue value)
        {
            if (value is GraphQLStringValue str)
            {
                return ParseValue(str.Value);
            }
            return NullableCursor(null);
        }
    }
}
