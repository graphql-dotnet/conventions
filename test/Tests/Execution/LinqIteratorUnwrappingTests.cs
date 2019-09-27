using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Tests;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Execution
{
    public class LinqIteratorUnwrappingTests
    {
        [Test]
        public void Schema_Will_Execute_With_No_Errors_When_A_Type_Is_In_A_Linq_Iterator()
        {
            const string query = @"{
                testSelectIterator
                testWhereIterator
            }";

            var schema = SchemaBuilderHelpers.Schema<BugReproSchemaTaskFirst>();
            var result = schema.Execute((e) => e.Query = query);
            ResultHelpers.AssertNoErrorsInResult(result);
            string testSelectIterator = (string)JObject.Parse(result)["data"]["testSelectIterator"][0];
            string testWhereIterator = (string)JObject.Parse(result)["data"]["testWhereIterator"][0];

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
                new NonNull<IEnumerable<NonNull<string>>>(new[] { new NonNull<string>("Test") }.Where(x => 1 == 1));
        }
    }
}
