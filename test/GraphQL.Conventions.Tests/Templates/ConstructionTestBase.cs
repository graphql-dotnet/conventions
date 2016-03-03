using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Builders;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Types;
using Xunit;

namespace GraphQL.Conventions.Tests.Templates
{
    public abstract class ConstructionTestBase : TestBase
    {
        protected ISchema Schema<TSchema>()
        {
            var typeAdapter = new GraphTypeAdapter();
            var constructor = new SchemaConstructor<ISchema, IGraphType>(typeAdapter);
            var schema = constructor.Build(typeof(TSchema));
            Assert.NotNull(schema);
            return schema;
        }

        protected ISchema Schema<TSchema1, TSchema2>()
        {
            var typeAdapter = new GraphTypeAdapter();
            var constructor = new SchemaConstructor<ISchema, IGraphType>(typeAdapter);
            var schema = constructor.Build(typeof(TSchema1), typeof(TSchema2));
            Assert.NotNull(schema);
            return schema;
        }

        protected ISchema Schema(GraphSchemaInfo schemaInfo)
        {
            var graphTypeAdapter = new GraphTypeAdapter();
            return graphTypeAdapter.DeriveSchema(schemaInfo);
        }

        protected IGraphType Type(GraphTypeInfo typeInfo)
        {
            var graphTypeAdapter = new GraphTypeAdapter();
            return graphTypeAdapter.DeriveType(typeInfo);
        }

        protected TCast Type<TCast>(GraphTypeInfo typeInfo)
            where TCast : class, IGraphType
        {
            var graphType = Type(typeInfo) as TCast;
            Assert.NotNull(graphType);
            return graphType;
        }

        protected IGraphType Type<TType>()
        {
            var type = Type(TypeInfo<TType>());
            Assert.NotNull(type);
            return type;
        }

        protected ObjectGraphType OutputType<TType>()
        {
            var type = Type<TType>() as ObjectGraphType;
            Assert.NotNull(type);
            return type;
        }

        protected InputObjectGraphType InputType<TType>()
        {
            var type = Type<TType>() as InputObjectGraphType;
            Assert.NotNull(type);
            return type;
        }
    }
}