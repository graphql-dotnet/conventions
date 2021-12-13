using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.DataLoader;
using GraphQL.NewtonsoftJson;
using Tests.Templates;

namespace Tests.Execution
{
    public class DataLoaderResultUnwrappingTests : ConstructionTestBase
    {
        [Test]
        public async Task Schema_Will_Execute_With_No_Errors_When_A_Type_Is_In_A_IDataLoaderResult()
        {
            const string Query = @"{
                dataLoaderResult
                nonNullDataLoaderResult
                nonNullListDataLoaderResult
                nestedDataLoaderResult
                nestedNonNullDataLoaderResult
                anEnum
                nestedEnum
                namedResult
                resultProperty
            }";

            var schema = Schema<BugReproSchemaDataLoaderResult>();

            var result = await schema.ExecuteAsync((e) => e.Query = Query);
            ResultHelpers.AssertNoErrorsInResult(result);
        }

        private class BugReproSchemaDataLoaderResult
        {
            public BugReproQueryDataLoaderResult Query { get; }
        }

        public enum BugReproQueryDataLoaderResultEnum
        {
            Zero,
            One
        }

        private class BugReproQueryDataLoaderResult
        {
            public IDataLoaderResult<string> DataLoaderResult() => new DataLoaderResult<string>("Test");

            public IDataLoaderResult<NonNull<string>> NonNullDataLoaderResult() => new DataLoaderResult<NonNull<string>>("Test");

            public IDataLoaderResult<NonNull<List<NonNull<string>>>> NonNullListDataLoaderResult() => new DataLoaderResult<NonNull<List<NonNull<string>>>>(new List<NonNull<string>>() { "Test" });

            public IDataLoaderResult<IDataLoaderResult<string>> NestedDataLoaderResult() => new DataLoaderResult<IDataLoaderResult<string>>(new DataLoaderResult<string>("Test"));

            public IDataLoaderResult<IDataLoaderResult<NonNull<string>>> NestedNonNullDataLoaderResult() => new DataLoaderResult<IDataLoaderResult<NonNull<string>>>(new DataLoaderResult<NonNull<string>>("Test"));

            public IDataLoaderResult<BugReproQueryDataLoaderResultEnum> AnEnum() => new DataLoaderResult<BugReproQueryDataLoaderResultEnum>(BugReproQueryDataLoaderResultEnum.One);

            public IDataLoaderResult<IDataLoaderResult<BugReproQueryDataLoaderResultEnum>> NestedEnum() => new DataLoaderResult<IDataLoaderResult<BugReproQueryDataLoaderResultEnum>>(new DataLoaderResult<BugReproQueryDataLoaderResultEnum>(BugReproQueryDataLoaderResultEnum.One));

            [Name("namedResult")]
            public IDataLoaderResult<int> ANamedResult() => new DataLoaderResult<int>(100500);

            public IDataLoaderResult<int> ResultProperty => new DataLoaderResult<int>(100500);

            public IDataLoaderResult<int> ResultField = new DataLoaderResult<int>(100500);
        }
    }
}
