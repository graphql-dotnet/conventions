using System;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types.Relay;
using GraphQL.Conventions.Types.Utilities;
using Xunit;

namespace GraphQL.Conventions.Tests.Types.Relay
{
    public class CursorTests : TestBase
    {
        [Fact]
        public void Can_Instantiate_Cursor_From_Encoded_String()
        {
            var cursor = new Cursor("VGVzdDoxMjM0NQ==");
            cursor.CursorForType<Test>().ShouldEqual("12345");
        }

        [Fact]
        public void Cannot_Instantiate_Cursor_From_Encoded_String_Of_Wrong_Type()
        {
            var cursor = new Cursor("VGVzdDoxMjM0NQ==");
            Assert.Throws<ArgumentException>(() => cursor.CursorForType<AnotherTest>());
        }

        [Fact]
        public void Cannot_Instantiate_Cursor_From_Empty_String()
        {
            Assert.Throws<ArgumentException>(() => new Cursor(null));
            Assert.Throws<ArgumentException>(() => new Cursor(string.Empty));
        }

        [Fact]
        public void Cannot_Instantiate_Cursor_From_Invalid_Base64_String()
        {
            Assert.Throws<ArgumentException>(() => new Cursor("abcdef"));
        }

        [Fact]
        public void Can_Sort_Cursors_Serialized_Using_Colon_Separators()
        {
            var cursor0 = Cursor.New<IdTests>("12345", true);
            var cursor1 = Cursor.New<IdTests>("12345", true);
            var cursor2 = Cursor.New<IdTests>("1235", true);
            var cursor3 = Cursor.New<IdTests>("99231", true);
            var cursor4 = Cursor.New<TestBase>("99231", true);

            Assert.True(Identifier.Decode(cursor0.ToString()).Contains(":"));
            Assert.True(cursor0 == cursor1);
            Assert.True(cursor0 <= cursor1);
            Assert.True(cursor0 >= cursor1);
            Assert.False(cursor0 < cursor1);
            Assert.False(cursor0 > cursor1);
            Assert.True(cursor1 > cursor2);
            Assert.True(cursor1 >= cursor2);
            Assert.True(cursor2 < cursor3);
            Assert.True(cursor2 <= cursor3);
            Assert.True(cursor3 < cursor4);
            Assert.True(cursor3 <= cursor4);
            Assert.False(cursor3 == cursor4);
            Assert.True(cursor3 != cursor4);
        }

        [Fact]
        public void Can_Sort_Identifiers_Serialized_Without_Using_Colon_Separators()
        {
            var cursor0 = Cursor.New<IdTests>("12345", false);
            var cursor1 = Cursor.New<IdTests>("12345", false);
            var cursor2 = Cursor.New<IdTests>("1235", false);
            var cursor3 = Cursor.New<IdTests>("99231", false);
            var cursor4 = Cursor.New<TestBase>("99231", false);

            Assert.False(Identifier.Decode(cursor0.ToString()).Contains(":"));
            Assert.True(cursor0 == cursor1);
            Assert.True(cursor0 <= cursor1);
            Assert.True(cursor0 >= cursor1);
            Assert.False(cursor0 < cursor1);
            Assert.False(cursor0 > cursor1);
            Assert.True(cursor1 > cursor2);
            Assert.True(cursor1 >= cursor2);
            Assert.True(cursor2 < cursor3);
            Assert.True(cursor2 <= cursor3);
            Assert.True(cursor3 < cursor4);
            Assert.True(cursor3 <= cursor4);
            Assert.False(cursor3 == cursor4);
            Assert.True(cursor3 != cursor4);
        }

        class Test
        {
        }

        class AnotherTest
        {
        }
    }
}
