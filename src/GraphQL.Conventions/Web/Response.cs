using System.Collections.Generic;
using System.Linq;
using GraphQL.NewtonsoftJson;

namespace GraphQL.Conventions.Web
{
    public class Response
    {
        private static readonly GraphQLSerializer Serializer = new GraphQLSerializer();

        private string _body;

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

        public Request Request { get; }

        public string QueryId => Request?.QueryId;

        public ExecutionResult ExecutionResult { get; }

        public Validation.IValidationResult ValidationResult { get; }

        public string GetBody()
        {
            if (string.IsNullOrWhiteSpace(_body) && ExecutionResult != null)
                _body = Serializer
                    .Serialize(ExecutionResult);

            return _body;
        }

        internal void SetBody(string value) => _body = value;

        public void AddExtra(string key, object value)
        {
            if (ExecutionResult.Extensions == null)
            {
                ExecutionResult.Extensions = new Dictionary<string, object>();
            }
            ExecutionResult.Extensions[key] = value;
            _body = null;
        }

        public bool HasData => ExecutionResult?.Data != null;

        public bool HasErrors => (Errors?.Any() ?? false) || (Warnings?.Any() ?? false);

        public bool IsValid => ValidationResult != null ? ValidationResult.IsValid : !HasErrors;

        public List<ExecutionError> Errors { get; } = new List<ExecutionError>();

        public List<ExecutionError> Warnings { get; } = new List<ExecutionError>();
    }
}
