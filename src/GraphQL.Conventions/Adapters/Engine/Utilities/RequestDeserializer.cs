using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public class RequestDeserializer : IRequestDeserializer
    {
        public QueryInput GetQueryFromRequestBody(string requestBody)
        {
            var request = ConvertJsonToDictionary(requestBody);
            var query = new QueryInput
            {
                QueryString = string.Empty,
            };

            if (request == null)
            {
                throw new ArgumentException($"Unable to deserialize JSON '{requestBody}'.");
            }

            if (!request.TryGetValue("query", out var queryString) && request.TryGetValue("data", out var data))
            {
                request = data as Dictionary<string, object>;
            }

            if (request?.TryGetValue("query", out queryString) ?? false)
            {
                query.QueryString = queryString as string ?? string.Empty;
            }

            object operationName = null;
            if (request?.TryGetValue("operationName", out operationName) ?? false)
            {
                query.OperationName = operationName as string;
            }

            object variables = null;
            if (request?.TryGetValue("variables", out variables) ?? false)
            {
                var variablesString = variables as string;
                if (!string.IsNullOrWhiteSpace(variablesString))
                {
                    variables = ConvertJsonToDictionary(variablesString);
                }

                if (variables is Dictionary<string, object> variablesDictionary)
                {
                    query.Variables = variablesDictionary;
                }
            }
            object extensions = null;
            if (request?.TryGetValue("extensions", out extensions) ?? false)
            {
                var extensionsString = extensions as string;
                if (!string.IsNullOrWhiteSpace(extensionsString))
                {
                    extensions = ConvertJsonToDictionary(extensionsString);
                }

                if (extensions is Dictionary<string, object> extensionsDictionary)
                {
                    query.Extensions = extensionsDictionary;
                }
            }

            return query;
        }

        public Dictionary<string, object> ConvertJsonToDictionary(string json)
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
            var values = JsonConvert.DeserializeObject(json, settings);
            return GetValue(values) as Dictionary<string, object>;
        }

        private static object GetValue(object value)
        {
            if (value is JObject objectValue)
            {
                var output = new Dictionary<string, object>();
                foreach (var kvp in objectValue)
                {
                    output.Add(kvp.Key, GetValue(kvp.Value));
                }
                return output;
            }

            if (value is JProperty propertyValue)
            {
                return new Dictionary<string, object>
                {
                    { propertyValue.Name, GetValue(propertyValue.Value) }
                };
            }

            if (value is JArray arrayValue)
            {
                return arrayValue.Children().Aggregate(new List<object>(), (list, token) =>
                {
                    list.Add(GetValue(token));
                    return list;
                });
            }

            if (value is JValue rawValue)
            {
                var val = rawValue.Value;
                if (val is long)
                {
                    long l = (long)val;
                    if (l >= int.MinValue && l <= int.MaxValue)
                    {
                        return (int)l;
                    }
                }
                return val;
            }

            return value;
        }
    }
}
