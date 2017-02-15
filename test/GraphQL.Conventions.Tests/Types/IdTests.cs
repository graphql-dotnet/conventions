using System;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types.Utilities;
using static GraphQL.Conventions.Utilities;
using Xunit;

namespace GraphQL.Conventions.Tests.Types
{
    public class IdTests : TestBase
    {
        [Fact]
        public void Can_Instantiate_Identifier_From_Encoded_String()
        {
            var id = Id("VGVzdDoxMjM0NQ==");
            id.IdentifierForType<Test>().ShouldEqual("12345");
        }

        [Fact]
        public void Cannot_Instantiate_Identifier_From_Encoded_String_Of_Wrong_Type()
        {
            var id = Id("VGVzdDoxMjM0NQ==");
            Assert.Throws<ArgumentException>(() => id.IdentifierForType<AnotherTest>());
        }

        [Fact]
        public void Cannot_Instantiate_Identifier_From_Empty_String()
        {
            Assert.Throws<ArgumentException>(() => Id(null));
            Assert.Throws<ArgumentException>(() => Id(string.Empty));
            var nullableId = (Id?)null;
            nullableId.HasValue.ShouldEqual(false);
        }

        [Fact]
        public void Cannot_Instantiate_Identifier_From_Invalid_Base64_String()
        {
            Assert.Throws<ArgumentException>(() => Id("abcdef"));
        }

        [Fact]
        public void Can_Sort_Identifiers_Serialized_Using_Colon_Separators()
        {
            var id0 = Id<IdTests>("12345", true);
            var id1 = Id<IdTests>("12345", true);
            var id2 = Id<IdTests>("1235", true);
            var id3 = Id<IdTests>("99231", true);
            var id4 = Id<TestBase>("99231", true);

            Assert.True(Identifier.Decode(id0.ToString()).Contains(":"));
            Assert.True(id0 == id1);
            Assert.True(id0 <= id1);
            Assert.True(id0 >= id1);
            Assert.False(id0 < id1);
            Assert.False(id0 > id1);
            Assert.True(id1 > id2);
            Assert.True(id1 >= id2);
            Assert.True(id2 < id3);
            Assert.True(id2 <= id3);
            Assert.True(id3 < id4);
            Assert.True(id3 <= id4);
            Assert.False(id3 == id4);
            Assert.True(id3 != id4);
        }

        [Fact]
        public void Can_Sort_Identifiers_Serialized_Without_Using_Colon_Separators()
        {
            var id0 = Id<IdTests>("12345", false);
            var id1 = Id<IdTests>("12345", false);
            var id2 = Id<IdTests>("1235", false);
            var id3 = Id<IdTests>("99231", false);
            var id4 = Id<TestBase>("99231", false);

            Assert.False(Identifier.Decode(id0.ToString()).Contains(":"));
            Assert.True(id0 == id1);
            Assert.True(id0 <= id1);
            Assert.True(id0 >= id1);
            Assert.False(id0 < id1);
            Assert.False(id0 > id1);
            Assert.True(id1 > id2);
            Assert.True(id1 >= id2);
            Assert.True(id2 < id3);
            Assert.True(id2 <= id3);
            Assert.True(id3 < id4);
            Assert.True(id3 <= id4);
            Assert.False(id3 == id4);
            Assert.True(id3 != id4);
        }

        [Fact]
        public void Can_Decode_Identifiers_Unambiguously_When_Serialized_With_Colon()
        {
            Id<TestItem>("12345").IsIdentifierForType<Test>()
                .ShouldBeFalse("Identifier for type 'TestItem' thought to be an identifier for type 'Test'.");
            Id<Test>("12345").IsIdentifierForType<TestItem>()
                .ShouldBeFalse("Identifier for type 'Test' thought to be an identifier for type 'TestItem'.");
        }

        [Fact]
        public void Can_Decode_Identifiers_Unambiguously_When_Serialized_Without_Colon()
        {
            Id<TestItem>("12345", false).IsIdentifierForType<Test>()
                .ShouldEqual(true); // Cannot distinguish between the two as it could either be: {'Test', 'Item12345'} or {'TestItem', '12345'}
            Id<Test>("12345", false).IsIdentifierForType<TestItem>()
                .ShouldBeFalse("Identifier for type 'Test' thought to be an identifier for type 'TestItem'.");
        }

        [Fact]
        public void Cannot_Decode_Empty_Identifiers()
        {
            Assert.Throws<ArgumentException>(() => Id<Test>("", true).IdentifierForType<Test>());
            Assert.Throws<ArgumentException>(() => Id<Test>("", false).IdentifierForType<Test>());
            Assert.Throws<ArgumentException>(() => Id("VGVzdDo=").IdentifierForType<Test>());
            Assert.Throws<ArgumentException>(() => Id("VGVzdA==").IdentifierForType<Test>());
        }

        class Test
        {
        }

        class AnotherTest
        {
        }

        class TestItem
        {
        }
    }
}
