using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public abstract class WrapperBase : IWrapper
    {
        protected WrapperBase(IWrapper parent)
        {
            _parent = parent;
        }

        protected WrapperBase()
        {
        }

        protected readonly IWrapper _parent;

        protected WrapperBase NextWrapper { get; private set; }

        public WrapperBase Next(WrapperBase unwrapper)
        {
            NextWrapper = unwrapper;
            return NextWrapper;
        }
        public object Wrap(GraphTypeInfo type, object value)
        {
            value = WrapValue(type, value);
            return NextWrapper != null
                ? NextWrapper.Wrap(type, value)
                : value;
        }

        public abstract object WrapValue(GraphTypeInfo type, object value);
    }
}