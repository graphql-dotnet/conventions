using System;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types.Utilities;
using Xunit;

namespace GraphQL.Conventions.Tests.Types
{
    public class IdTests : TestBase
    {
        [Fact]
        public void Can_Instantiate_Identifier_From_Encoded_String()
        {
            var id = new Id("VGVzdDoxMjM0NQ==");
            id.IdentifierForType<Test>().ShouldEqual("12345");
        }

        [Fact]
        public void Cannot_Instantiate_Identifier_From_Encoded_String_Of_Wrong_Type()
        {
            var id = new Id("VGVzdDoxMjM0NQ==");
            Assert.Throws<ArgumentException>(() => id.IdentifierForType<AnotherTest>());
        }

        [Fact]
        public void Cannot_Instantiate_Identifier_From_Empty_String()
        {
            Assert.Throws<ArgumentException>(() => new Id(null));
            Assert.Throws<ArgumentException>(() => new Id(string.Empty));
            var nullableId = (Id?)null;
            nullableId.HasValue.ShouldEqual(false);
        }

        [Fact]
        public void Cannot_Instantiate_Identifier_From_Invalid_Base64_String()
        {
            Assert.Throws<ArgumentException>(() => new Id("abcdef"));
        }

        [Fact]
        public void Can_Sort_Identifiers_Serialized_Using_Colon_Separators()
        {
            var id0 = Id.New<IdTests>("12345", true);
            var id1 = Id.New<IdTests>("12345", true);
            var id2 = Id.New<IdTests>("1235", true);
            var id3 = Id.New<IdTests>("99231", true);
            var id4 = Id.New<TestBase>("99231", true);

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
            var id0 = Id.New<IdTests>("12345", false);
            var id1 = Id.New<IdTests>("12345", false);
            var id2 = Id.New<IdTests>("1235", false);
            var id3 = Id.New<IdTests>("99231", false);
            var id4 = Id.New<TestBase>("99231", false);

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

        class Test
        {
        }

        class AnotherTest
        {
        }
    }
}
