using System;
using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Language.AST;
using GraphQL.Types;

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

        public override object ParseLiteral(IValue value)
        {
            if (value is StringValue str)
            {
                return ParseValue(str.Value);
            }
            return null;
        }
    }
}
