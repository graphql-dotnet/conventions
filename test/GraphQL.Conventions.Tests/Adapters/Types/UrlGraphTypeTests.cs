using System;
using GraphQL.Conventions.Adapters.Types;
using GraphQL.Conventions.Types;
using GraphQL.Language.AST;
using Xunit;

namespace GraphQL.Conventions.Tests.Adapters.Types
{
    public class UrlGraphTypeTests : ScalarTypeTestBase<UrlGraphType, string, Url>
    {
        [Fact]
        public override void Can_Serialize()
        {
            ShouldSerialize(null, null);
            ShouldSerialize(new Url("http://www.google.com/"), "http://www.google.com/");
            ShouldSerialize(new Url("mailto:someone@somewhere.com"), "mailto:someone@somewhere.com");
        }

        [Fact]
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

        [Fact]
        public override void Can_Parse_Literal()
        {
            ShouldParseLiteral(new StringValue(null), null);
            ShouldParseLiteral(new StringValue("http://www.google.com/"), new Url("http://www.google.com/"));
            ShouldParseLiteral(new StringValue("mailto:someone@somewhere.com"), new Url("mailto:someone@somewhere.com"));
            ShouldParseLiteral(new StringValue("\"http://www.google.com/\""), new Url("http://www.google.com/"));
            ShouldThrow<UriFormatException>(() => ShouldParseLiteral(new StringValue("www.google.com"), null));
            ShouldThrow<ArgumentException>(() => ShouldParseLiteral(new StringValue("mp4:af93420c0dff"), null));
            ShouldThrow<UriFormatException>(() => ShouldParseLiteral(new StringValue("\"\"http://www.google.com/\"\""), null));
            ShouldParseLiteral(new IntValue(0), null);
        }
    }
}
