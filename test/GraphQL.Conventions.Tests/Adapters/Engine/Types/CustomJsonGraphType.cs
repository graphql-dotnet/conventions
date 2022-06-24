using System.Collections.Generic;
using GraphQL.Types;
using GraphQLParser.AST;

// ReSharper disable InconsistentNaming

namespace Tests.Adapters.Engine.Types
{
    #region .NET Class
    public class JSON : GraphQLValue
    {
        public IDictionary<string, object> Value { get; protected set; }

        public override string ToString() => $"{GetType().Name}{{value={Value}}}";

        public JSON()
        {

        }

        public JSON(IDictionary<string, object> value)
        {
            Value = value;
        }

        public override ASTNodeKind Kind { get => ASTNodeKind.Field; }
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

        /*
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
        public override object ParseLiteral(GraphQLValue value)
        {
            var jsonValue = value as JSON;
            return jsonValue?.Value;
        }

        public override IValue ToAST(object value)
        {
            return new JSON(value as IDictionary<string, object>);
        }
        */
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

        public override object ParseLiteral(GraphQLValue value)
        {
            var jsonValue = value as JSON;
            return jsonValue?.Value;
        }
    }
    #endregion
}
