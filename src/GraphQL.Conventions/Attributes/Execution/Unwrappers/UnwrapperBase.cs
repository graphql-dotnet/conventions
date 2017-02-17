namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public abstract class UnwrapperBase : IUnwrapper
    {
        protected UnwrapperBase(IUnwrapper parent)
        {
            _parent = parent;
        }

        protected UnwrapperBase()
        {
        }

        protected readonly IUnwrapper _parent;

        protected UnwrapperBase NextUnwrapper { get; private set; }

        public UnwrapperBase Next(UnwrapperBase unwrapper)
        {
            NextUnwrapper = unwrapper;
            return NextUnwrapper;
        }

        public object Unwrap(object value)
        {
            value = UnwrapValue(value);
            return NextUnwrapper != null
                ? NextUnwrapper.Unwrap(value)
                : value;
        }

        public abstract object UnwrapValue(object value);
    }
}