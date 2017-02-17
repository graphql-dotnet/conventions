using System.Collections.Generic;

namespace GraphQL.Conventions
{
    public interface IRequestDeserializer
    {
        QueryInput GetQueryFromRequestBody(string requestBody);

        Dictionary<string, object> ConvertJsonToDictionary(string json);
    }
}
