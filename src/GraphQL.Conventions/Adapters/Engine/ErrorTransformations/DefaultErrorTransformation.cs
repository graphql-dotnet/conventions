using System;
using System.Linq;
using GraphQL.Conventions.Execution;
using GraphQLParser;

namespace GraphQL.Conventions.Adapters.Engine.ErrorTransformations
{
    [Obsolete("Please use AddErrorInfoProvider or ConfigureExecution to transform returned errors. You can also use ConfigureExecutionOptions and set the UnhandledExceptionDelegate property.")]
    public class DefaultErrorTransformation : IErrorTransformation
    {
        public ExecutionErrors Transform(ExecutionErrors errors)
            => errors.Aggregate(new ExecutionErrors(), (result, executionError) =>
            {
                var exception = new FieldResolutionException(executionError);
                var error = new ExecutionError(exception.Message, exception);
                foreach (var location in executionError.Locations ?? Enumerable.Empty<Location>())
                {
                    error.AddLocation(location);
                }
                error.Path = executionError.Path;
                result.Add(error);
                return result;
            });
    }
}
