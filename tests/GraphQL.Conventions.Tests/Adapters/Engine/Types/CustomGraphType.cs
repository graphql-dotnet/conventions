using GraphQL.Types;
using GraphQLParser.AST;

namespace Tests.Adapters.Engine.Types
{
    public class CustomGraphType : ScalarGraphType
    {
        public CustomGraphType()
        {
            Name = "Custom";
            Description = "A custom scalar type (example).";
        }

        public override object Serialize(object value)
        {
            return $"CUSTOM:{value}";
        }

        public override object ParseValue(object value)
        {
            return new Custom
            {
                Value = value?.ToString()?.Replace("\"", "").Replace("CUSTOM:", ""),
            };
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

    public class Custom
    {
        public string Value { get; set; }

        public override string ToString() =>
            Value;
    }
}
