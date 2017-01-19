using System;
using System.Collections.Generic;
using GraphQL.Conventions.Adapters.Engine;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Tests.Adapters.Engine.Types;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types;
using Xunit;

namespace GraphQL.Conventions.Tests.Adapters.Engine
{
    public class DependencyInjectionTests : TestBase
    {
        [Fact]
        public void Can_Construct_And_Describe_Schema_With_Injections()
        {
            var engine = new GraphQLEngine();
            engine.BuildSchema(typeof(SchemaDefinition<Query>));
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            type Query {
              field: String
            }
            ");
        }

        [Fact]
        public async void Can_Execute_Query_On_Schema_With_Injections()
        {
            var engine = new GraphQLEngine();
            engine.BuildSchema(typeof(SchemaDefinition<Query>));
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ field }")
                .EnableValidation()
                .Execute();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("field", "Some Value");
        }

        // TODO DI for constructors and params
        // TODO Id<> and Cursor<> wrappers
        // TODO Connection<> extensions
        // TODO What about [NotNull]?

        class Query
        {
            private readonly IRepository _repository;

            public Query(IRepository repository)
            {
                _repository = repository;
            }

            public string Field => _repository.GetValue();
        }

        interface IRepository
        {
            string GetValue();
        }

        class Repository : IRepository
        {
            public string GetValue()
            {
                return "Some Value";
            }
        }
    }
}