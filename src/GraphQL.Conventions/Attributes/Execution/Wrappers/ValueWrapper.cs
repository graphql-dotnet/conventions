using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public class ValueWrapper : WrapperBase
    {
        public ValueWrapper()
        {
            Next(new ObjectWrapper(this))
                .Next(new CollectionWrapper(this))
                .Next(new OptionalWrapper())
                .Next(new NonNullWrapper())
                .Next(new PrimitiveWrapper());
        }

        public override object WrapValue(GraphEntityInfo entityInfo, GraphTypeInfo typeInfo, object value, bool isSpecified) => value;
    }
}
