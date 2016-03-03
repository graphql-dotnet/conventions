using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace GraphQL.Conventions.Adapters.Types
{
    public class UrlGraphType : ScalarGraphType
    {
        public UrlGraphType()
        {
            Name = TypeNames.Url;
            Description = "Uniform Resource Locator; reference (address) to a resource on the Internet.";
        }

        public override object Serialize(object value)
        {
            return value?.ToString();
        }

        public override object ParseValue(object value)
        {
            var url = value?.ToString().StripQuotes();
            return string.IsNullOrWhiteSpace(url)
                ? null
                : new Url(url);
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
