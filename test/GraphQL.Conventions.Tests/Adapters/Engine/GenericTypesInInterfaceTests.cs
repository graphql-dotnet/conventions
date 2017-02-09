using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using Xunit;

namespace GraphQL.Conventions.Tests.Adapters.Engine
{
    public class GenericTypesInInterfaceTests : TestBase
    {
        [Fact]
        public void Can_Construct_And_Describe_Schema_Using_Interfaces_With_Generic_Types()
        {
            var engine = GraphQLEngine.New<Query>();
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            type Account implements IAccount {
                id: Int!
            }
            interface IAccount {
                id: Int!
            }
            type Query {
                account: IAccount
            }
            ");
        }

        [Fact]
        public async void Can_Execute_Query_On_Schema_Using_Interfaces_With_Generic_Types()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ account { id } }")
                .EnableValidation()
                .Execute();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("account", "id", 123);
        }

        interface IEntity<T>
        {
            T Id { get; }
        }

        interface IAccount : IEntity<int> { }

        class Account : IAccount
        {
            public int Id { get; } = 123;
        }

        class Query
        {
            public IAccount Account { get; } = new Account();
        }
    }
}