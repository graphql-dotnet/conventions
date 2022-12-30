using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Web
{
    public class GraphQLBuilderTests
    {
        [Test]
        public async Task Can_Run_Query()
        {
            int schemaRunCount = 0;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGraphQL(b => b
                .AddConventionsSchema<TestQuery>()
                .AddSystemTextJson()
                .ConfigureSchema(_ => schemaRunCount++));

            var services = serviceCollection.BuildServiceProvider();

            var executer = services.GetRequiredService<IDocumentExecuter<ISchema>>();
            var result = await executer.ExecuteAsync(new ExecutionOptions
            {
                Query = "{ hello }",
                RequestServices = services,
            });
            var serializer = services.GetRequiredService<IGraphQLTextSerializer>();
            var body = serializer.Serialize(result);
            Assert.AreEqual("{\"data\":{\"hello\":\"World\"}}", body);
            Assert.AreEqual(1, schemaRunCount);
        }

        private class TestQuery
        {
            public string Hello => "World";
        }
    }
}
