using System.Threading.Tasks;
using GraphQL.NewtonsoftJson;
using Tests.Templates;
using Tests.Templates.Extensions;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Tests.Execution
{
    public class SchemaExecutionTests : ConstructionTestBase
    {
        [Test]
        public async Task Can_Have_Decimals_In_Schema()
        {
            var schema = Schema<SchemaTypeWithDecimal>();
            schema.ShouldHaveQueries(1);
            schema.ShouldHaveMutations(0);
            schema.Query.ShouldHaveFieldWithName("test");
            var result = await schema.ExecuteAsync((e) => e.Query = "query { test }");
            ResultHelpers.AssertNoErrorsInResult(result);
        }

        private class SchemaTypeWithDecimal
        {
            public QueryTypeWithDecimal Query { get; }
        }

        private class QueryTypeWithDecimal
        {
            public decimal Test => 10;
        }
    }
}
