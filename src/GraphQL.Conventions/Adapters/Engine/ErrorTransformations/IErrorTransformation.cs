using System;

namespace GraphQL.Conventions.Adapters.Engine.ErrorTransformations
{
    [Obsolete("Please use AddErrorInfoProvider or ConfigureExecution to transform returned errors. You can also use ConfigureExecutionOptions and set the UnhandledExceptionDelegate property.")]
    public interface IErrorTransformation
    {
        ExecutionErrors Transform(ExecutionErrors errors);
    }
}
