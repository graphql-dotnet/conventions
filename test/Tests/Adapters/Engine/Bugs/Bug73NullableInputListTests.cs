using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Conventions.Tests.Adapters.Engine.Types;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Validation.Complexity;

namespace GraphQL.Conventions.Tests.Adapters.Engine.Bugs
{
    public class Bug73NullableInputListTests : TestBase
    {
        [Test]
        public async void Can_Accept_Null_List_From_Literal()
        {
            var engine = GraphQLEngine
                .New<TestQuery>();
            var result = await engine
                .NewExecutor()
                .WithQueryString(@"query _ { example(testInputs:null) }")
                .Execute();
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("example", "null");
        }

        [Test]
        public async void Can_Accept_Null_List_From_Input()
        {
            var engine = GraphQLEngine
                .New<TestQuery>();
            var result = await engine
                .NewExecutor()
                .WithQueryString(@"query _($inputs:[TestInput]) { example(testInputs:$inputs) }")
                .WithInputs(new Dictionary<string, object>
                {
                    { "inputs", null} ,
                })
                .Execute();
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("example", "null");
        }
    }

    public class TestQuery
    {
        public string Example(IEnumerable<TestInput> testInputs)
        {
            return testInputs == null
                ? "null"
                : "[" + string.Join(",", testInputs.Select(x => x == null ? "null" : x.Text)) + "]";
        }
    }

    [InputType]
    public class TestInput
    {
        public string Text { get; set; }
    }
}