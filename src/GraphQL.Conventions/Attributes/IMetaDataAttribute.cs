using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes
{
    public interface IMetaDataAttribute : IAttribute
    {
        bool ShouldBeApplied(GraphEntityInfo entity);

        void DeriveMetaData(GraphEntityInfo entity);
    }
}
