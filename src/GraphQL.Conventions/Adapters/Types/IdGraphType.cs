using GraphQL.Conventions.Adapters.Types.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Language.AST;
using GraphQL.Types;
using static GraphQL.Conventions.Utilities;

namespace GraphQL.Conventions.Adapters.Types
{
    public class IdGraphType : ScalarGraphType
    {
        public IdGraphType() 
        {
            Name = TypeNames.Identity;
        }

        public override object Serialize(object value)
        {
            return value?.ToString();
        }

        public override object ParseValue(object value)
        {
            var id = value?.ToString().StripQuotes();
            return NullableId(id);
        }

        public override object ParseLiteral(IValue value)
        {
            if (value is StringValue str)
            {
                return ParseValue(str.Value);
            }
            return NullableId(null);
        }
    }
}
