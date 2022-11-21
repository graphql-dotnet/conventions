using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
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

        class SchemaTypeWithDecimal
        {
            public QueryType Query { get; }

            public class QueryType
            {
                public decimal Test => 10;
            }
        }


        /// <summary>
        /// This tests a specific egde-case: <br/>
        /// A query that selects __typename directly on a field (and not in an inline fragment) with an argument and that returns an union.
        /// </summary>
        [Test]
        public async Task Can_Select_Typename_On_Union_With_Argument()
        {
            string query = @"query {
                test(useTypeA: true) {
                    __typename
                    ... on TypeA {
                        id
                    }
                }
            }";

            var schema = Schema<SchemaTypeComplexUnion>();

            schema.ShouldHaveQueries(1);
            schema.ShouldHaveMutations(0);
            schema.Query.ShouldHaveFieldWithName("test");

            var result = await schema.ExecuteAsync((e) => e.Query = query);
                
            ResultHelpers.AssertNoErrorsInResult(result);
        }

        class SchemaTypeComplexUnion 
        {
            public QueryType Query { get; }

            public class QueryType
            {
                public NonNull<ExampleUnion> test(bool useTypeA) {
                    if (useTypeA) {
                        return new ExampleUnion(new TypeA() {
                            id = "typeA-id"
                        });
                    } else {
                        return new ExampleUnion(new TypeB() {
                            name = "typeB-name"
                        });
                    }
                }

                public class ExampleUnion : Union<TypeA, TypeB>
                {
                    public ExampleUnion(TypeA a)
                    {
                        base.Instance = a;
                    }
                    public ExampleUnion(TypeB b)
                    {
                        base.Instance = b;
                    }
                }

                public class TypeA
                {
                    public string id { get; set; } = null;
                }

                public class TypeB
                {
                    public string name { get; set; }= null;
                }
            }
        }
    }
}
