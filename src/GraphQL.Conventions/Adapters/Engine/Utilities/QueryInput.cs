using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public class QueryInput
    {
        public string QueryString { get; set; }

        public string OperationName { get; set; }

        public Dictionary<string, object> Extensions { get; set; }

        public Dictionary<string, object> Variables { get; set; }
    }
}
