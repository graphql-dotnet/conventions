using System;
using GraphQL.Conventions.Adapters.Types;
using GraphQLParser.AST;

namespace Tests.Adapters.Types
{
    public class TimeSpanGraphTypeTests : ScalarTypeTestBase<TimeSpanGraphType, string, TimeSpan?>
    {
        [Test]
        public override void Can_Serialize()
        {
            ShouldSerialize(null, null);
            ShouldSerialize(new TimeSpan(10, 20, 30, 40, 50), "10.20:30:40.0500000");
            ShouldSerialize(new TimeSpan(5, 10, 20, 30), "5.10:20:30");
            ShouldSerialize(new TimeSpan(5, 10, 20), "05:10:20");
        }

        [Test]
        public override void Can_Parse_Value()
        {
            ShouldParseValue("10.20:30:40.0500000", new TimeSpan(10, 20, 30, 40, 50));
            ShouldParseValue("10.20:30:40.05", new TimeSpan(10, 20, 30, 40, 50));
            ShouldParseValue("5.10:20:30", new TimeSpan(5, 10, 20, 30));
            ShouldParseValue("05:10:20", new TimeSpan(5, 10, 20));
            ShouldParseValue("\"05:10:20\"", new TimeSpan(5, 10, 20));
        }

        [Test]
        public override void Can_Parse_Literal()
        {
            ShouldParseLiteral(new GraphQLStringValue("10.20:30:40.0500000"), new TimeSpan(10, 20, 30, 40, 50));
            ShouldParseLiteral(new GraphQLStringValue("10.20:30:40.05"), new TimeSpan(10, 20, 30, 40, 50));
            ShouldParseLiteral(new GraphQLStringValue("5.10:20:30"), new TimeSpan(5, 10, 20, 30));
            ShouldParseLiteral(new GraphQLStringValue("05:10:20"), new TimeSpan(5, 10, 20));
            ShouldParseLiteral(new GraphQLIntValue(0), null);
        }
    }
}
