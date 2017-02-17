using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Adapters
{
    public interface IGraphTypeAdapter<TSchemaType, TGraphType>
    {
        TSchemaType DeriveSchema(GraphSchemaInfo schemaInfo);

        TGraphType DeriveType(GraphTypeInfo typeInfo);

        void RegisterScalarType<TType>(string name);
    }
}
