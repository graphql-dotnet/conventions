using System.Collections.Generic;
using GraphQL.Language.AST;
using GraphQL.Types;

// ReSharper disable InconsistentNaming

namespace Tests.Adapters.Engine.Types
{
    #region GraphTypeConverter
    public class JsonGraphTypeConverter : IAstFromValueConverter
    {
        public bool Matches(object value, IGraphType type)
        {
            return type.Name == nameof(JSON);
        }

        public IValue Convert(object value, IGraphType type)
        {
            return new JSON(value as IDictionary<string, object>);
        }
    }
    #endregion

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

        protected override bool Equals(ValueNode<IDictionary<string, object>> node)
        {
            return Value == node.Value;
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
    }
    #endregion
}
