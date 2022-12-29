using System;
using GraphQL.Conventions.Adapters.Types;
using GraphQLParser.AST;

namespace Tests.Adapters.Types
{
    public class UriGraphTypeTests : ScalarTypeTestBase<UriGraphType, string, Uri>
    {
        [Test]
        public override void Can_Serialize()
        {
            ShouldSerialize(null, null);
            ShouldSerialize(new Uri("http://www.google.com/"), "http://www.google.com/");
            ShouldSerialize(new Uri("mailto:someone@somewhere.com"), "mailto:someone@somewhere.com");
            ShouldSerialize(new Uri("mp4:af93420c0dff"), "mp4:af93420c0dff");
        }

        [Test]
        public override void Can_Parse_Value()
        {
            ShouldParseValue(null, null);
            ShouldParseValue("http://www.google.com/", new Uri("http://www.google.com/"));
            ShouldParseValue("mailto:someone@somewhere.com", new Uri("mailto:someone@somewhere.com"));
            ShouldParseValue("\"http://www.google.com/\"", new Uri("http://www.google.com/"));
            ShouldParseValue("mp4:af93420c0dff", new Uri("mp4:af93420c0dff"));
            ShouldThrow<UriFormatException>(() => ShouldParseValue("www.google.com", null));
            ShouldThrow<UriFormatException>(() => ShouldParseValue("\"\"http://www.google.com/\"\"", null));
        }

        [Test]
        public override void Can_Parse_Literal()
        {
            ShouldParseLiteral(new GraphQLNullValue(), null);
            ShouldParseLiteral(new GraphQLStringValue("http://www.google.com/"), new Uri("http://www.google.com/"));
            ShouldParseLiteral(new GraphQLStringValue("mailto:someone@somewhere.com"), new Uri("mailto:someone@somewhere.com"));
            ShouldParseLiteral(new GraphQLStringValue("\"http://www.google.com/\""), new Uri("http://www.google.com/"));
            ShouldParseLiteral(new GraphQLStringValue("mp4:af93420c0dff"), new Uri("mp4:af93420c0dff"));
            ShouldThrow<UriFormatException>(() => ShouldParseLiteral(new GraphQLStringValue("www.google.com"), null));
            ShouldThrow<UriFormatException>(() => ShouldParseLiteral(new GraphQLStringValue("\"\"http://www.google.com/\"\""), null));
            ShouldParseLiteral(new GraphQLIntValue(0), null);
        }
    }
}
