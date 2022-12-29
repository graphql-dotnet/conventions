using System.Reflection;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Utilities;
using Tests.Templates;
using Tests.Templates.Extensions;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Tests.Adapters.Engine
{
    public class DynamicConstructionTests : TestBase
    {
        [Test]
        public async Task Can_Construct_And_Describe_Schema_With_Dynamic_Queries()
        {
            var typeAdapter = new GraphTypeAdapter();
            var typeResolver = new TypeResolver();

            var userRepositoryInterface = GetGraphType(typeAdapter, typeResolver, typeof(IUserRepository).GetTypeInfo());
            var userRepositoryType = GetGraphType(typeAdapter, typeResolver, typeof(UserRepository).GetTypeInfo());
            var userType = GetGraphType(typeAdapter, typeResolver, typeof(User).GetTypeInfo());

            var schema = new Schema
            {
                Query = new ObjectGraphType { Name = "Query" }
            };

            schema.RegisterTypes(userType, userRepositoryInterface, userRepositoryType);

            schema.Query.AddField(new FieldType
            {
                Name = "users",
                ResolvedType = userRepositoryType,
                Resolver = new CustomResolver(new UserRepository()),
            });

            var schemaDescription = new SchemaPrinter(schema).Print();
            schemaDescription.ShouldEqualWhenReformatted(@"
            interface IUserRepository {
                getUserById(id: String): User
            }
            type Query {
                users: UserRepository
            }
            type User {
                id: String
                name: String
            }
            type UserRepository implements IUserRepository {
                getUserById(id: String): User
            }
            ");

            var executer = new DocumentExecuter();
            var result = await executer.ExecuteAsync(new ExecutionOptions
            {
                Schema = schema,
                Query = "{ users { getUserById(id: \"1\") { id name } } }",
            });

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("users", "getUserById", "id", "1");
            result.Data.ShouldHaveFieldWithValue("users", "getUserById", "name", "User #1");
        }

        private IGraphType GetGraphType(GraphTypeAdapter typeAdapter, TypeResolver typeResolver, TypeInfo typeInfo)
        {
            var graphTypeInfo = typeResolver.DeriveType(typeInfo);
            return typeAdapter.DeriveType(graphTypeInfo);
        }

        private class CustomResolver : IFieldResolver
        {
            private readonly UserRepository _userRepository;

            public CustomResolver(UserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public ValueTask<object> ResolveAsync(IResolveFieldContext context)
            {
                return new ValueTask<object>(_userRepository);
            }
        }

        private class User
        {
            public string Id { get; set; }

            public string Name { get; set; }
        }

        private interface IUserRepository
        {
            User GetUserById(string id);
        }

        private class UserRepository : IUserRepository
        {
            public User GetUserById(string id) =>
                new User
                {
                    Id = id,
                    Name = $"User #{id}",
                };
        }
    }
}
