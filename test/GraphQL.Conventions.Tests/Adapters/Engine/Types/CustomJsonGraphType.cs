using System.Collections.Generic;
using GraphQL.Language.AST;
using GraphQL.Types;

// ReSharper disable InconsistentNaming

namespace Tests.Adapters.Engine.Types
{
    #region .NET Class
    public class JSON : ValueNode<IDictionary<string, object>>
    {
        public JSON()
        {

        }

        public JSON(IDictionary<string, object> value)
        {
            Value = value;
        }
    }
    #endregion

    #region ScalarGraphType
    public class JSONScalarGraphType : ScalarGraphType
    {
        public JSONScalarGraphType()
        {
            Name = nameof(JSON);
            Description = "Untyped JSON Structure";
        }


        public override object Serialize(object value)
        {
            return ParseValue(value);
        }

        public override object ParseValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is JSON json)
            {
                return json.Value;
            }

            return value;
        }
        public override object ParseLiteral(IValue value)
        {
            var jsonValue = value as JSON;
            return jsonValue?.Value;
        }

        public override IValue ToAST(object value)
        {
            return new JSON(value as IDictionary<string, object>);
        }
    }
    #endregion
}
