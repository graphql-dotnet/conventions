namespace GraphQLParser.Tests
{
    using GraphQLParser;
    using NUnit.Framework;

    [TestFixture]
    public class SourceTests
    {
        [Test]
        public void CreateSourceFromString_BodyEqualsToProvidedSource()
        {
            var source = new Source("somesrc");

            Assert.AreEqual("somesrc", source.Body);
        }

        [Test]
        public void CreateSourceFromString_SourceNameEqualsToGraphQL()
        {
            var source = new Source("somesrc");

            Assert.AreEqual("GraphQL", source.Name);
        }

        [Test]
        public void CreateSourceFromStringWithName_SourceNameEqualsToProvidedName()
        {
            var source = new Source("somesrc", "somename");

            Assert.AreEqual("somename", source.Name);
        }
    }
}