using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions;
using GraphQL.Conventions.Web;

namespace GraphQL.Conventions.Extensions
{
    public static class ProfilingResultEnricher
    {
        public static void EnrichWithProfilingInformation(this Response response)
        {
            var perf = response?.ExecutionResult?.Perf;
            if (perf == null) { return; }

            var records = new List<PerformanceRecord>();
            foreach (var record in perf)
            {
                if (record.Category != "field") { continue; }

                records.Add(new PerformanceRecord
                {
                    Path = record.Metadata.ContainsKey("path") ? string.Join(".", record.Metadata["path"] as List<string>) : null,
                    StartTimeInMs = (long)record.Start,
                    EndTimeInMs = (long)record.End,
                    ParentType = GetOrDefault<string>(record.Metadata, "typeName", null),
                    Field = GetOrDefault<string>(record.Metadata, "fieldName", null),
                    Arguments = GetOrDefault<Dictionary<string, object>>(record.Metadata, "arguments", null),
                });
            }

            if (records.Any())
            {
                response.AddExtra("profile", records.OrderBy(record => record.Path));
            }
        }

        private static T GetOrDefault<T>(Dictionary<string, object> dictionary, string key, T defaultValue)
            where T : class
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key] as T;
            }
            return defaultValue;
        }
    }
}
