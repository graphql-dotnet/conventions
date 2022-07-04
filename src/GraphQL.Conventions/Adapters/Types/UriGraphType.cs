using System;
using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.Conventions.Adapters.Types
{
    public class UriGraphType : ScalarGraphType
    {
        public UriGraphType()
        {
            Name = TypeNames.Uri;
            Description = "Uniform Resource Identifier.";
        }

        public override object Serialize(object value)
        {
            return value?.ToString();
        }

        public override object ParseValue(object value)
        {
            var uri = value?.ToString().StripQuotes();
            return string.IsNullOrWhiteSpace(uri)
                ? null
                : new Uri(uri);
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
