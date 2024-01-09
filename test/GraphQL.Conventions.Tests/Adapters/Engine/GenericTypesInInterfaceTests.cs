using System.Threading.Tasks;
using GraphQL.Conventions;
using Tests.Templates;
using Tests.Templates.Extensions;

namespace Tests.Adapters.Engine
{
    public class GenericTypesInInterfaceTests : TestBase
    {
        [Test]
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

        [Test]
        public async Task Can_Execute_Query_On_Schema_Using_Interfaces_With_Generic_Types()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ account { id } }")
                .EnableValidation()
                .ExecuteAsync();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("account", "id", 123);
        }

        private interface IEntity<T>
        {
            T Id { get; }
        }

        private interface IAccount : IEntity<int> { }

        private class Account : IAccount
        {
            public int Id { get; } = 123;
        }

        private class Query
        {
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public IAccount Account { get; } = new Account();
        }
    }
}
