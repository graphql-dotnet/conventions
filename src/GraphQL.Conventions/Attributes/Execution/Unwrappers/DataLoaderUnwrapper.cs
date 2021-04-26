using GraphQL.DataLoader;

namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public class DataLoaderUnwrapper : UnwrapperBase
    {
        private static readonly ValueUnwrapper _valueUnwrapper = new ValueUnwrapper();

        public override object UnwrapValue(object value)
        {
            if (value is IDataLoaderResult dataLoaderResult)
            {
                return new SimpleDataLoader<object>(async cancellationToken =>
                {
                    var result = await dataLoaderResult.GetResultAsync(cancellationToken).ConfigureAwait(false);
                    return _valueUnwrapper.Unwrap(result);
                });
            }
            else
                return value;
        }
    }
}