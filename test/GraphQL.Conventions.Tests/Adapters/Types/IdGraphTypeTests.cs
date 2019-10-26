using System;
using GraphQL.Conventions.Adapters.Types;
using GraphQL.Conventions.Types.Utilities;
using GraphQL.Language.AST;

namespace GraphQL.Conventions.Tests.Adapters.Types
{
    public class IdGraphTypeTests : ScalarTypeTestBase<IdGraphType, string, Id?>
    {
        private static readonly int _id = 12345;

        private static readonly string _serializedId = Identifier.Encode($"{nameof(Foo)}:{_id}");

        [Test]
        public override void Can_Serialize()
        {
            ShouldSerialize(null, null);
            ShouldSerialize(Id.New<Foo>(_id), _serializedId);
            ShouldThrow<ArgumentException>(() => ShouldSerialize($"WRONG{_serializedId}", null));
        }

        [Test]
        public override void Can_Parse_Value()
        {
            ShouldParseValue(null, null);
            ShouldParseValue(_serializedId, Id.New<Foo>(_id));
            ShouldThrow<ArgumentException>(() => ShouldParseValue($"WRONG{_serializedId}", null));
        }

        [Test]
        public override void Can_Parse_Literal()
        {
            ShouldParseLiteral(new StringValue(null), null);
            ShouldParseLiteral(new StringValue(_serializedId), Id.New<Foo>(_id));
            ShouldThrow<ArgumentException>(() => ShouldParseLiteral(new StringValue($"WRONG{_serializedId}"), null));
            ShouldParseLiteral(new IntValue(0), null);
        }

        class Foo
        {
        }
    }
}
