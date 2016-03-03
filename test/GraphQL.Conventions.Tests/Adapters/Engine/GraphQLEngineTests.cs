using System;
using System.Collections.Generic;
using GraphQL.Conventions.Adapters.Engine;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types;
using Xunit;

namespace GraphQL.Conventions.Tests.Adapters.Engine
{
    public class GraphQLEngineTests : TestBase
    {
        [Fact]
        public void Can_Construct_And_Describe_Basic_Schema()
        {
            var engine = new GraphQLEngine(typeof(SchemaDefinition<BasicQuery>));
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            type BasicQuery {
              booleanField1: Boolean
              booleanField2: Boolean!
              dateField1: Date
              dateField2: Date!
              doubleField1: Float
              doubleField2: Float!
              floatField1: Float
              floatField2: Float!
              fooField1: Foo
              fooField2: Foo!
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

        [Fact]
        public void Can_Construct_And_Describe_Polymorphic_Schema()
        {
            var engine = new GraphQLEngine(typeof(SchemaDefinition<Query>));
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            interface ISemanticVersion {
              majorVersion: Int!
              minorVersion: Int!
              revision: Int!
            }
            type Actor {
              dateOfBirth: Date
              firstName: String
              lastName: String!
            }
            type ExtendedVersion implements ISemanticVersion {
              branchName: String!
              majorVersion: Int!
              minorVersion: Int!
              revision: Int!
            }
            type Movie {
              actors: [Actor]
              releaseDate: Date
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
            union SearchResultItem = Movie | Actor
            ");
        }

        class BasicQuery
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
        }

        class Foo
        {
            public Id Id => Id.New<Foo>("12345");
        }

        class Query
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

        interface ISemanticVersion
        {
            int MajorVersion { get; }

            int MinorVersion { get; }

            int Revision { get; }
        }

        class ExtendedVersion : ISemanticVersion
        {
            public int MajorVersion { get; set; }

            public int MinorVersion { get; set; }

            public int Revision { get; set; }

            public NonNull<string> BranchName { get; set; }
        }

        class Movie
        {
            public NonNull<string> Title { get; set; }

            public List<Actor> Actors { get; set; }

            public DateTime? ReleaseDate { get; set; }
        }

        class Actor
        {
            public string FirstName { get; set; }

            public NonNull<string> LastName { get; set; }

            public DateTime? DateOfBirth { get; set; }
        }

        class SearchResultItem : Union<Movie, Actor>
        {
        }

        class SearchResult
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
    }
}