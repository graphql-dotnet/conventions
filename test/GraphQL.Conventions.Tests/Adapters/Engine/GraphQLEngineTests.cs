using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.Validation.Complexity;
using Tests.Adapters.Engine.Types;
using Tests.Templates;
using Tests.Templates.Extensions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable InconsistentNaming

namespace Tests.Adapters.Engine
{
    public class GraphQLEngineTests : TestBase
    {
        [Test]
        public void Can_Construct_And_Describe_Basic_Schema()
        {
            var engine = GraphQLEngine.New<BasicQuery>();
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            schema {
                query: BasicQuery
            }
            type BasicQuery {
                booleanField1: Boolean
                booleanField2: Boolean!
                dateField1: DateTime
                dateField2: DateTime!
                doubleField1: Float
                doubleField2: Float!
                floatField1: Float
                floatField2: Float!
                fooField1: Foo
                fooField2: Foo!
                guidField1: GUID
                guidField2: GUID!
                intField1: Int
                intField2: Int!
                stringField1: String
                stringField2: String!
                timeSpanField1: TimeSpan
                timeSpanField2: TimeSpan!
                urlField1: URL
                urlField2: URL!
            }
            type Foo {
                id: ID!
            }
            ");
        }

        [Test]
        public void Can_Construct_And_Describe_Polymorphic_Schema()
        {
            var engine = GraphQLEngine.New<Query>();
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
                type Actor {
                  dateOfBirth: DateTime
                  firstName: String
                  lastName: String!
                }

                scalar DateTime

                type ExtendedVersion implements ISemanticVersion {
                  branchName: String!
                  majorVersion: Int!
                  minorVersion: Int!
                  revision: Int!
                }

                interface ISemanticVersion {
                  majorVersion: Int!
                  minorVersion: Int!
                  revision: Int!
                }

                type Movie {
                  actors: [Actor]
                  releaseDate: DateTime
                  title: String!
                }

                type Query {
                  search(searchFor: String!): [SearchResult]
                  version(branchName: String): ISemanticVersion
                }

                type SearchResult {
                  node: SearchResultItem
                  score: Float!
                }

                union SearchResultItem = Actor | Movie
            ");
        }

        [Test]
        public void Can_Construct_And_Describe_Schema_With_Enums()
        {
            var engine = GraphQLEngine.New<QueryWithEnums>();
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            schema {
                query: QueryWithEnums
            }
            enum Enum1 {
                OPTION1
                OPTION2
                OPTION3
            }
            input InputObject {
                anotherField: RenamedEnum
                someField: RenamedEnum = SOME_VALUE1
                yetAnotherDummyField: RenamedEnum = SOME_VALUE2
                yetAnotherField: RenamedEnum!
            }
            type QueryWithEnums {
                field1: Enum1!
                field2: RenamedEnum
                field3(arg1: RenamedEnum, arg2: Enum1!, arg3: RenamedEnum = SOME_VALUE2, arg4: Enum1 = OPTION3): RenamedEnum!
                field4(input: InputObject): RenamedEnum
                field5(arg: Enum1): Enum1
            }
            enum RenamedEnum {
                SOME_VALUE1
                SOME_VALUE2
            }
            ");
        }

        [Test]
        public async Task Can_Evaluate_Enums_Arguments_With_Default_Values()
        {
            var executor = GraphQLEngine
                .New<QueryWithEnums>()
                .NewExecutor();
            var result = await executor
                .WithQueryString(@"
                {
                    field3(arg2: OPTION3)
                    field4(input: { yetAnotherField: SOME_VALUE1 })
                    field5
                }")
                .ExecuteAsync();

            result.Data.ShouldHaveFieldWithValue("field3", "SOME_VALUE2");
            result.Data.ShouldHaveFieldWithValue("field4", "SOME_VALUE2");
            result.Data.ShouldHaveFieldWithValue("field5", null);
        }

        [Test]
        public void Can_Construct_And_Describe_Schema_With_Interfaces()
        {
            var engine = GraphQLEngine.New<QueryWithInterfaces>();
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            schema {
                query: QueryWithInterfaces
            }
            interface Interface1 {
                field1: String
            }
            interface Interface2 {
                field2: String
            }
            type QueryWithInterfaces {
                field: TypeFromTwoInterfaces
            }
            type TypeFromTwoInterfaces implements Interface1 & Interface2 {
                field1: String
                field2: String
            }
            ");
        }

        [Test]
        public async Task Can_Register_And_Use_Custom_Scalar_Types()
        {
            var engine = GraphQLEngine
                .New()
                .RegisterScalarType<Custom, CustomGraphType>()
                .WithQuery<CustomTypesQuery>();
            var result = await engine
                .NewExecutor()
                .WithQueryString(@"{ customScalarType(arg:""CUSTOM:Test"") }")
                .ExecuteAsync();
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("customScalarType", "CUSTOM:WRAPPED:Test");
        }

        [Test]
        public async Task Can_Register_And_Use_Custom_Json_Scalar_Types()
        {
            var engine = GraphQLEngine
                .New()
                .RegisterScalarType<JSON, JSONScalarGraphType>()
                .WithQuery<CustomJsonTypeQuery>();
            var result = await engine
                .NewExecutor()
                .WithQueryString(@"{ customJsonScalarType }")
                .ExecuteAsync();
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("customJsonScalarType", new Dictionary<string, object> { { "test", true } });
        }

        [Test]
        public async Task Can_Run_Simple_Query_Using_ComplexityOptions()
        {
            var executor = GraphQLEngine
                .New<Movie>()
                .NewExecutor();

            var result = await executor
                .WithComplexityOptions(new ComplexityOptions { MaxDepth = 1 })
                .WithQueryString(@"
                    {
                        title
                        releaseDate
                    }")
                .ExecuteAsync();

            result.Data.ShouldHaveFieldWithValue("title", "Movie 1");
        }

        [Test]
        public async Task Cannot_Run_Too_Complex_Query_Using_ComplexityConfiguration()
        {
            var executor = GraphQLEngine
                .New<Movie>()
                .NewExecutor();

            var result = await executor
                .WithComplexityOptions(new ComplexityOptions { MaxDepth = 1 })
                .WithQueryString(@"
                    {
                        title
                        releaseDate
                        actors{
                            firstName
                            lastName
                        }
                    }")
                .ExecuteAsync();

            result.ShouldHaveErrors(1);
            var error = result.Errors.First().InnerException?.ToString();
            error.ShouldContainWhenReformatted("Query is too nested to execute. Maximum depth is 2 levels; maximum allowed on this endpoint is 1.");
        }

        private class BasicQuery
        {
            public bool? BooleanField1 { get; }

            public bool BooleanField2 { get; }

            public DateTime? DateField1 { get; }

            public DateTime DateField2 { get; }

            public double? DoubleField1 { get; }

            public double DoubleField2 { get; }

            public float? FloatField1 { get; }

            public float FloatField2 { get; }

            public Foo FooField1 { get; }

            public NonNull<Foo> FooField2 { get; }

            public int? IntField1 { get; }

            public int IntField2 { get; }

            public string StringField1 { get; }

            public NonNull<string> StringField2 { get; }

            public TimeSpan? TimeSpanField1 { get; }

            public TimeSpan TimeSpanField2 { get; }

            public Url UrlField1 { get; }

            public NonNull<Url> UrlField2 { get; }

            public Guid? GuidField1 { get; }

            public Guid GuidField2 { get; }
        }

        private class Foo
        {
            public Id Id => Id.New<Foo>("12345");
        }

        private class Query
        {
            public ISemanticVersion Version(string branchName) =>
                new ExtendedVersion
                {
                    MajorVersion = 1,
                    BranchName = branchName ?? "master",
                };

            public List<SearchResult> Search(NonNull<string> searchFor) =>
                new List<SearchResult>
                {
                    new SearchResult(0.97, new Movie()),
                };
        }

        private interface ISemanticVersion
        {
            int MajorVersion { get; }

            int MinorVersion { get; }

            int Revision { get; }
        }

        private class ExtendedVersion : ISemanticVersion
        {
            public int MajorVersion { get; set; }

            public int MinorVersion { get; set; }

            public int Revision { get; set; }

            public NonNull<string> BranchName { get; set; }
        }

        private class Movie
        {
            public NonNull<string> Title { get; set; } = "Movie 1";

            public List<Actor> Actors { get; set; }

            public DateTime? ReleaseDate { get; set; }
        }

        private class Actor
        {
            public string FirstName { get; set; }

            public NonNull<string> LastName { get; set; }

            public DateTime? DateOfBirth { get; set; }
        }

        private class SearchResultItem : Union<Movie, Actor>
        {
        }

        private class SearchResult
        {
            public SearchResult(double score, Movie movie)
            {
                Score = score;
                Node = new SearchResultItem { Instance = movie };
            }

            public SearchResult(double score, Actor actor)
            {
                Score = score;
                Node = new SearchResultItem { Instance = actor };
            }

            public double Score { get; set; }

            public SearchResultItem Node { get; set; }
        }

        private class CustomJsonTypeQuery
        {
            public JSON CustomJsonScalarType()
            {
                return new JSON(new Dictionary<string, object> { { "test", true } });
            }
        }

        private class CustomTypesQuery
        {
            public Custom CustomScalarType(Custom arg) =>
                new Custom { Value = $"WRAPPED:{arg.Value}" };
        }

        private class QueryWithEnums
        {
            public Enum1 Field1 => Enum1.Option1;

            public Enum2? Field2 => Enum2.SomeValue1;

            public Enum2 Field3(Enum2? arg1, Enum1 arg2, Enum2? arg3 = Enum2.SomeValue2, Enum1? arg4 = Enum1.Option3) => arg3 ?? Enum2.SomeValue1;

            public Enum2? Field4(InputObject input) => input.YetAnotherDummyField;

            public Enum1? Field5(Enum1? arg = null) => arg;
        }

        private enum Enum1
        {
            Option1,
            Option2,
            Option3,
        }

        [Name("RenamedEnum")]
        private enum Enum2
        {
            SomeValue1,
            SomeValue2,
        }

        [InputType]
        private class InputObject
        {
            [DefaultValue(Enum2.SomeValue1)]
            public Enum2? SomeField { get; set; }

            public Enum2? AnotherField { get; set; }

            public Enum2 YetAnotherField { get; set; }

            [DefaultValue(Enum2.SomeValue2)]
            public Enum2? YetAnotherDummyField { get; set; }
        }

        private class QueryWithInterfaces
        {
            public TypeFromTwoInterfaces Field => null;
        }

        private interface Interface1
        {
            string Field1 { get; }
        }

        private interface Interface2
        {
            string Field2 { get; }
        }

        private class TypeFromTwoInterfaces : Interface1, Interface2
        {
            public string Field1 => string.Empty;

            public string Field2 => string.Empty;
        }
    }
}
