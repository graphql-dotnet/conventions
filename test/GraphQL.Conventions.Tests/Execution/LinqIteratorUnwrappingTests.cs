using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.NewtonsoftJson;
using Newtonsoft.Json.Linq;
using Tests.Templates;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Tests.Execution
{
    public class LinqIteratorUnwrappingTests : ConstructionTestBase
    {
        [Test]
        public async Task Schema_Will_Execute_With_No_Errors_When_A_Type_Is_In_A_Linq_Iterator()
        {
            const string query = @"{
                testSelectIterator
                testWhereIterator
            }";

            var schema = Schema<BugReproSchemaTaskFirst>();

            var result = await schema.ExecuteAsync((e) => e.Query = query);
            ResultHelpers.AssertNoErrorsInResult(result);
            string testSelectIterator = (string)JObject.Parse(result)["data"]?["testSelectIterator"]?[0];
            string testWhereIterator = (string)JObject.Parse(result)["data"]?["testWhereIterator"]?[0];

            Assert.AreEqual("Test", testSelectIterator);
            Assert.AreEqual("Test", testWhereIterator);
        }

        private class BugReproSchemaTaskFirst
        {
            public BugReproQueryTaskFirst Query { get; }
        }

        private class BugReproQueryTaskFirst
        {
            public NonNull<IEnumerable<NonNull<string>>> TestSelectIterator() =>
                new NonNull<IEnumerable<NonNull<string>>>(new[] { "Test" }.Select(x => new NonNull<string>(x)));

            public NonNull<IEnumerable<NonNull<string>>> TestWhereIterator() =>
                new NonNull<IEnumerable<NonNull<string>>>(new[] { new NonNull<string>("Test") }.Where(x => x.Value != null));
        }
    }
}
