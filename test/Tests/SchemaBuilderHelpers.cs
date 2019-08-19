using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Builders;
using GraphQL.Types;

namespace GraphQL.Conventions.Tests
{
    public static class SchemaBuilderHelpers
    {
        public static ISchema Schema<TSchema>()
        {
            var typeAdapter = new GraphTypeAdapter();
            var constructor = new SchemaConstructor<ISchema, IGraphType>(typeAdapter);
            var schema = constructor.Build(typeof(TSchema));
            Assert.IsNotNull(schema);
            return schema;
        }
    }
}
