using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Utilities;

namespace GraphQL.Conventions.Tests.Adapters.Engine
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

            var executer = new GraphQL.DocumentExecuter();
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

        class CustomResolver : IFieldResolver
        {
            private readonly UserRepository _userRepository;

            public CustomResolver(UserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public object Resolve(IResolveFieldContext context) =>
                _userRepository;
        }

        class User
        {
            public string Id { get; set; }

            public string Name { get; set; }
        }

        interface IUserRepository
        {
            User GetUserById(string id);
        }

        class UserRepository : IUserRepository
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