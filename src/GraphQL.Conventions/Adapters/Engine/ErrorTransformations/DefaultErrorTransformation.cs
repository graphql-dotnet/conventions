using System.Linq;
using GraphQL.Conventions.Execution;

namespace GraphQL.Conventions.Adapters.Engine.ErrorTransformations
{
    public class DefaultErrorTransformation : IErrorTransformation
    {
        public ExecutionErrors Transform(ExecutionErrors errors)
            => errors.Aggregate(new ExecutionErrors(), (result, executionError) =>
            {
                var exception = new FieldResolutionException(executionError);
                var error = new ExecutionError(exception.Message, exception);
                foreach (var location in executionError.Locations ?? Enumerable.Empty<ErrorLocation>())
                {
                    error.AddLocation(location.Line, location.Column);
                }
                error.Path = executionError.Path;
                result.Add(error);
                return result;
            });
    }
}
