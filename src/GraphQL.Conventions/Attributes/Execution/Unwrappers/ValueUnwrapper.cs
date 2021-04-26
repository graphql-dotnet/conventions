namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public class ValueUnwrapper : UnwrapperBase
    {
        public ValueUnwrapper()
        {
            this.Next(new NonNullUnwrapper())
                .Next(new OptionalUnwrapper())
                .Next(new UnionUnwrapper())
                .Next(new CollectionUnwrapper(this))
                .Next(new DataLoaderUnwrapper());
        }

        public override object UnwrapValue(object value) => value;
    }
}