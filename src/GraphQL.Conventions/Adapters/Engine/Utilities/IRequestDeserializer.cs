using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public interface IRequestDeserializer
    {
        QueryInput GetQueryFromRequestBody(string requestBody);

        Dictionary<string, object> ConvertJsonToDictionary(string json);
    }
}
