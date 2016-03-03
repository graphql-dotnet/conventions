using System;
using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Language.AST;
using GraphQL.Types;

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
