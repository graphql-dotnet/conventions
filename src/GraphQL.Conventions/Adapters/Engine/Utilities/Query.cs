using System.Collections.Generic;

namespace GraphQL.Conventions.Adapters.Engine.Utilities
{
    public class Query
    {
        public string QueryString { get; set; }

        public string OperationName { get; set; }

        public Dictionary<string, object> Variables { get; set; }
    }
}
