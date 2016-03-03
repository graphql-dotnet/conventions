using System.Collections.Generic;

namespace GraphQL.Conventions.Adapters.Engine.Utilities
{
    public interface IRequestDeserializer
    {
        Query GetQueryFromRequestBody(string requestBody);

        Dictionary<string, object> ConvertJsonToDictionary(string json);
    }
}
