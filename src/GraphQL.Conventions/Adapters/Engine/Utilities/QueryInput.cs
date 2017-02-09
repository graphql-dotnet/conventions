using System.Collections.Generic;

namespace GraphQL.Conventions
{
    public class QueryInput
    {
        public string QueryString { get; set; }

        public string OperationName { get; set; }

        public Dictionary<string, object> Variables { get; set; }
    }
}
