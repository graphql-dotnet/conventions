using System;
using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.Conventions.Adapters.Types
{
    public class GuidGraphType : ScalarGraphType
    {
        public GuidGraphType()
        {
            Name = TypeNames.Guid;
            Description = "Globally Unique Identifier.";
        }

        public override object Serialize(object value)
        {
            return value?.ToString();
        }

        public override object ParseValue(object value)
        {
            var guid = value?.ToString().StripQuotes();
            return string.IsNullOrWhiteSpace(guid)
                ? null
                : (Guid?)Guid.Parse(guid);
        }

        public override object ParseLiteral(GraphQLValue value)
        {
            if (value is GraphQLStringValue str)
            {
                return ParseValue(str.Value);
            }
            return null;
        }
    }
}
