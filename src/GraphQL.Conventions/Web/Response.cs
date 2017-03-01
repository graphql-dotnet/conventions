using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Conventions.Web
{
    public class Response
    {
        public Response(
            Request request,
            ExecutionResult result,
            string serializedResult,
            List<Type> exceptionsTreatedAsWarnings = null)
        {
            Request = request;
            ExecutionResult = result;
            Body = serializedResult;
            PopulateErrorsAndWarnings(exceptionsTreatedAsWarnings);
        }

        public Response(
            Request request,
            Validation.IValidationResult result,
            List<Type> exceptionsTreatedAsWarnings = null)
        {
            Request = request;
            ValidationResult = result;
            PopulateErrorsAndWarnings(exceptionsTreatedAsWarnings);
        }

        private void PopulateErrorsAndWarnings(List<Type> exceptionsTreatedAsWarnings)
        {
            var errors = Errors?.Where(e => !string.IsNullOrWhiteSpace(e?.Message));
            foreach (var error in errors ?? new List<ExecutionError>())
            {
                if (exceptionsTreatedAsWarnings.Contains(error.InnerException.GetType()))
                {
                    Warnings.Add(error);
                }
                else
                {
                    Errors.Add(error);
                }
            }
        }

        public Request Request { get; private set; }

        public string QueryId => Request?.QueryId;

        public ExecutionResult ExecutionResult { get; private set; }

        public Validation.IValidationResult ValidationResult { get; private set; }

        public string Body { get; }

        public bool HasData => ExecutionResult?.Data != null;

        public bool HasErrors => (Errors?.Any() ?? false) || (Warnings?.Any() ?? false);

        public bool IsValid => ValidationResult != null ? ValidationResult.IsValid : !HasErrors;

        public IList<ExecutionError> Errors { get; } = new List<ExecutionError>();

        public IList<ExecutionError> Warnings { get; } = new List<ExecutionError>();
    }
}
