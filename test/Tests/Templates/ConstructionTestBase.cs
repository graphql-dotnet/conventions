using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Builders;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Types;

namespace GraphQL.Conventions.Tests.Templates
{
    public abstract class ConstructionTestBase : TestBase
    {
        protected ISchema Schema<TSchema>()
        {
            var typeAdapter = new GraphTypeAdapter();
            var constructor = new SchemaConstructor<ISchema, IGraphType>(typeAdapter);
            var schema = constructor.Build(typeof(TSchema));
            Assert.IsNotNull(schema);
            return schema;
        }

        protected ISchema Schema<TSchema1, TSchema2>()
        {
            var typeAdapter = new GraphTypeAdapter();
            var constructor = new SchemaConstructor<ISchema, IGraphType>(typeAdapter);
            var schema = constructor.Build(typeof(TSchema1), typeof(TSchema2));
            Assert.IsNotNull(schema);
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
            Assert.IsNotNull(graphType);
            return graphType;
        }

        protected IGraphType Type<TType>()
        {
            var type = Type(TypeInfo<TType>());
            Assert.IsNotNull(type);
            return type;
        }

        protected ObjectGraphType OutputType<TType>()
        {
            var type = Type<TType>() as ObjectGraphType;
            Assert.IsNotNull(type);
            return type;
        }

        protected InputObjectGraphType InputType<TType>()
        {
            var type = Type<TType>() as InputObjectGraphType;
            Assert.IsNotNull(type);
            return type;
        }
    }
}