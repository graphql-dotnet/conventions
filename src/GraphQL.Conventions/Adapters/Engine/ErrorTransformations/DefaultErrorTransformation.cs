using System.Threading.Tasks;
using GraphQL.Conventions.Execution;

namespace GraphQL.Conventions.Adapters.Engine.ErrorTransformations
{
    public class DefaultErrorTransformation : IErrorTransformation
    {
        public Task<ExecutionErrors> Transform(ExecutionErrors errors)
        {
            var result = new ExecutionErrors();
            foreach (var executionError in errors)
            {
                var exception = new FieldResolutionException(executionError);
                var error = new ExecutionError(exception.Message, exception);
                foreach (var location in executionError.Locations ?? new ErrorLocation[0])
                {
                    error.AddLocation(location.Line, location.Column);
                }
                error.Path = executionError.Path;
                result.Add(error);
            }

            return Task.FromResult(result);
        }
    }
}
