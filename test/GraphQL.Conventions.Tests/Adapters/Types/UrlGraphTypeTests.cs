using System;
using GraphQL.Conventions;
using GraphQL.Conventions.Adapters.Types;
using GraphQLParser.AST;

namespace Tests.Adapters.Types
{
    public class UrlGraphTypeTests : ScalarTypeTestBase<UrlGraphType, string, Url>
    {
        [Test]
        public override void Can_Serialize()
        {
            ShouldSerialize(null, null);
            ShouldSerialize(new Url("http://www.google.com/"), "http://www.google.com/");
            ShouldSerialize(new Url("mailto:someone@somewhere.com"), "mailto:someone@somewhere.com");
        }

        [Test]
        public override void Can_Parse_Value()
        {
            ShouldParseValue(null, null);
            ShouldParseValue("http://www.google.com/", new Url("http://www.google.com/"));
            ShouldParseValue("mailto:someone@somewhere.com", new Url("mailto:someone@somewhere.com"));
            ShouldParseValue("\"http://www.google.com/\"", new Url("http://www.google.com/"));
            ShouldThrow<UriFormatException>(() => ShouldParseValue("www.google.com", null));
            ShouldThrow<ArgumentException>(() => ShouldParseValue("mp4:af93420c0dff", null));
            ShouldThrow<UriFormatException>(() => ShouldParseValue("\"\"http://www.google.com/\"\"", null));
        }

        [Test]
        public override void Can_Parse_Literal()
        {
            ShouldParseLiteral(new GraphQLNullValue(), null);
            ShouldParseLiteral(new GraphQLStringValue("http://www.google.com/"), new Url("http://www.google.com/"));
            ShouldParseLiteral(new GraphQLStringValue("mailto:someone@somewhere.com"), new Url("mailto:someone@somewhere.com"));
            ShouldParseLiteral(new GraphQLStringValue("\"http://www.google.com/\""), new Url("http://www.google.com/"));
            ShouldThrow<UriFormatException>(() => ShouldParseLiteral(new GraphQLStringValue("www.google.com"), null));
            ShouldThrow<ArgumentException>(() => ShouldParseLiteral(new GraphQLStringValue("mp4:af93420c0dff"), null));
            ShouldThrow<UriFormatException>(() => ShouldParseLiteral(new GraphQLStringValue("\"\"http://www.google.com/\"\""), null));
            ShouldParseLiteral(new GraphQLIntValue(0), null);
        }
    }
}
