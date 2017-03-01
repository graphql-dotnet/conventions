using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Conventions.Web
{
    public class Response
    {
        public Response(
            Request request,
            ExecutionResult result)
        {
            Request = request;
            ExecutionResult = result;
        }

        public Response(
            Request request,
            Validation.IValidationResult result)
        {
            Request = request;
            ValidationResult = result;
        }

        public Request Request { get; private set; }

        public string QueryId => Request?.QueryId;

        public ExecutionResult ExecutionResult { get; private set; }

        public Validation.IValidationResult ValidationResult { get; private set; }

        public string Body { get; internal set; }

        public bool HasData => ExecutionResult?.Data != null;

        public bool HasErrors => (Errors?.Any() ?? false) || (Warnings?.Any() ?? false);

        public bool IsValid => ValidationResult != null ? ValidationResult.IsValid : !HasErrors;

        public IList<ExecutionError> Errors { get; } = new List<ExecutionError>();

        public IList<ExecutionError> Warnings { get; } = new List<ExecutionError>();
    }
}
