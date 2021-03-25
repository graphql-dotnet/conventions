using System;
using GraphQL.Conventions.Adapters.Types;
using GraphQL.Language.AST;

namespace Tests.Adapters.Types
{
    public class GuidGraphTests : ScalarTypeTestBase<GuidGraphType, string, Guid?>
    {
        [Test]
        public override void Can_Serialize()
        {
            ShouldSerialize(null, null);
            ShouldSerialize(new Guid("ad9da688-1fd4-4e00-ad89-b4d3fef08280"), "ad9da688-1fd4-4e00-ad89-b4d3fef08280");
            ShouldSerialize(new Guid("8b38981dad0f4f3ebbf197909f55c051"), "8b38981d-ad0f-4f3e-bbf1-97909f55c051");
        }

        [Test]
        public override void Can_Parse_Value()
        {
            ShouldParseValue(null, null);
            ShouldParseValue("ad9da688-1fd4-4e00-ad89-b4d3fef08280", new Guid("ad9da688-1fd4-4e00-ad89-b4d3fef08280"));
            ShouldParseValue("\"8b38981d-ad0f-4f3e-bbf1-97909f55c051\"", new Guid("8b38981dad0f4f3ebbf197909f55c051"));
        }

        [Test]
        public override void Can_Parse_Literal()
        {
            ShouldParseLiteral(new NullValue(), null);
            ShouldParseLiteral(new StringValue("ad9da688-1fd4-4e00-ad89-b4d3fef08280"), new Guid("ad9da688-1fd4-4e00-ad89-b4d3fef08280"));
            ShouldParseLiteral(new IntValue(0), null);
        }
    }
}
