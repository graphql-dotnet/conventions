using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            if (request.TryGetValue("query", out queryString))
            {
                query.QueryString = queryString as string ?? string.Empty;
            }

            if (request.TryGetValue("operationName", out var operationName))
            {
                query.OperationName = operationName as string;
            }

            if (request.TryGetValue("variables", out var variables))
            {
                var variablesString = variables as string;
                if (!string.IsNullOrWhiteSpace(variablesString))
                {
                    variables = ConvertJsonToDictionary(variablesString);
                }

                var variablesDictionary = variables as Dictionary<string, object>;
                if (variablesDictionary != null)
                {
                    query.Variables = variablesDictionary;
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
            var objectValue = value as JObject;
            if (objectValue != null)
            {
                var output = new Dictionary<string, object>();
                foreach (var kvp in objectValue)
                {
                    output.Add(kvp.Key, GetValue(kvp.Value));
                }
                return output;
            }

            var propertyValue = value as JProperty;
            if (propertyValue != null)
            {
                return new Dictionary<string, object>
                {
                    { propertyValue.Name, GetValue(propertyValue.Value) }
                };
            }

            var arrayValue = value as JArray;
            if (arrayValue != null)
            {
                return arrayValue.Children().Aggregate(new List<object>(), (list, token) =>
                {
                    list.Add(GetValue(token));
                    return list;
                });
            }

            var rawValue = value as JValue;
            if (rawValue != null)
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
