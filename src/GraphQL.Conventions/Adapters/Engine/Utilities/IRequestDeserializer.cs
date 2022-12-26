using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    [Obsolete("Please use a GraphQL serialization library such as GraphQL.SystemTextJson to deserialize JSON requests.")]
    public interface IRequestDeserializer
    {
        QueryInput GetQueryFromRequestBody(string requestBody);

        Dictionary<string, object> ConvertJsonToDictionary(string json);
    }
}
