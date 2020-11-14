using System;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Types.Utilities;
using Tests.Templates;
using Tests.Templates.Extensions;

// ReSharper disable ObjectCreationAsStatement

namespace Tests.Types.Relay
{
    public class CursorTests : TestBase
    {
        [Test]
        public void Can_Instantiate_Cursor_From_Encoded_String()
        {
            var cursor = new Cursor("VGVzdDoxMjM0NQ==");
            cursor.CursorForType<Test>().ShouldEqual("12345");
        }

        [Test]
        public void Cannot_Instantiate_Cursor_From_Encoded_String_Of_Wrong_Type()
        {
            var cursor = new Cursor("VGVzdDoxMjM0NQ==");
            Assert.ThrowsException<ArgumentException>(() => cursor.CursorForType<AnotherTest>());
        }

        [Test]
        public void Cannot_Instantiate_Cursor_From_Empty_String()
        {
            Assert.ThrowsException<ArgumentException>(() => new Cursor(null));
            Assert.ThrowsException<ArgumentException>(() => new Cursor(string.Empty));
        }

        [Test]
        public void Cannot_Instantiate_Cursor_From_Invalid_Base64_String()
        {
            Assert.ThrowsException<ArgumentException>(() => new Cursor("abcdef"));
        }

        [Test]
        public void Can_Sort_Cursors_Serialized_Using_Colon_Separators()
        {
            var cursor0 = Cursor.New<IdTests>("12345", true);
            var cursor1 = Cursor.New<IdTests>("12345", true);
            var cursor2 = Cursor.New<IdTests>("1235", true);
            var cursor3 = Cursor.New<IdTests>("99231", true);
            var cursor4 = Cursor.New<TestBase>("99231", true);

            Assert.IsTrue(Identifier.Decode(cursor0.ToString()).Contains(":"));
            Assert.IsTrue(cursor0 == cursor1);
            Assert.IsTrue(cursor0 <= cursor1);
            Assert.IsTrue(cursor0 >= cursor1);
            Assert.IsFalse(cursor0 < cursor1);
            Assert.IsFalse(cursor0 > cursor1);
            Assert.IsTrue(cursor1 > cursor2);
            Assert.IsTrue(cursor1 >= cursor2);
            Assert.IsTrue(cursor2 < cursor3);
            Assert.IsTrue(cursor2 <= cursor3);
            Assert.IsTrue(cursor3 < cursor4);
            Assert.IsTrue(cursor3 <= cursor4);
            Assert.IsFalse(cursor3 == cursor4);
            Assert.IsTrue(cursor3 != cursor4);
        }

        [Test]
        public void Can_Sort_Identifiers_Serialized_Without_Using_Colon_Separators()
        {
            var cursor0 = Cursor.New<IdTests>("12345", false);
            var cursor1 = Cursor.New<IdTests>("12345", false);
            var cursor2 = Cursor.New<IdTests>("1235", false);
            var cursor3 = Cursor.New<IdTests>("99231", false);
            var cursor4 = Cursor.New<TestBase>("99231", false);

            Assert.IsFalse(Identifier.Decode(cursor0.ToString()).Contains(":"));
            Assert.IsTrue(cursor0 == cursor1);
            Assert.IsTrue(cursor0 <= cursor1);
            Assert.IsTrue(cursor0 >= cursor1);
            Assert.IsFalse(cursor0 < cursor1);
            Assert.IsFalse(cursor0 > cursor1);
            Assert.IsTrue(cursor1 > cursor2);
            Assert.IsTrue(cursor1 >= cursor2);
            Assert.IsTrue(cursor2 < cursor3);
            Assert.IsTrue(cursor2 <= cursor3);
            Assert.IsTrue(cursor3 < cursor4);
            Assert.IsTrue(cursor3 <= cursor4);
            Assert.IsFalse(cursor3 == cursor4);
            Assert.IsTrue(cursor3 != cursor4);
        }

        class Test
        {
        }

        class AnotherTest
        {
        }
    }
}
