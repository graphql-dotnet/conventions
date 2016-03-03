using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Language.AST;
using GraphQL.Types;
using Xunit;

namespace GraphQL.Conventions.Tests.Adapters.Types
{
    public abstract class ScalarTypeTestBase<TType, TSerializedRepresentation, TNativeRepresentation> : TestBase
        where TType : ScalarGraphType, new()
    {
        private readonly ScalarGraphType _graphType = new TType();

        protected void ShouldSerialize(TNativeRepresentation data, TSerializedRepresentation expected)
        {
            _graphType.Serialize(data).ShouldEqual(expected);
        }

        protected void ShouldParseValue(TSerializedRepresentation data, TNativeRepresentation expected)
        {
            _graphType.ParseValue(data).ShouldEqual(expected);
        }

        protected void ShouldParseLiteral(IValue data, TNativeRepresentation expected)
        {
            _graphType.ParseLiteral(data).ShouldEqual(expected);
        }

        [Fact]
        public abstract void Can_Serialize();

        [Fact]
        public abstract void Can_Parse_Value();

        [Fact]
        public abstract void Can_Parse_Literal();
    }
}
