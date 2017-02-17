using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Conventions.Web
{
    public class Response
    {
        public Response(Request request, ExecutionResult result, string serializedResult)
        {
            Request = request;
            Result = result;
            Body = serializedResult;
        }

        public Request Request { get; private set; }

        public string QueryId => Request?.QueryId;

        public ExecutionResult Result { get; private set; }

        public string Body { get; }

        public bool HasData => Result.Data != null;

        public bool HasErrors => (Errors?.Any() ?? false) || (Warnings?.Any() ?? false);

        public IList<ExecutionError> Errors { get; } = new List<ExecutionError>();

        public IList<ExecutionError> Warnings { get; } = new List<ExecutionError>();
    }
}
