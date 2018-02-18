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
                    Path = string.Join(".", record.Metadata["path"] as List<string>),
                    StartTimeInMs = (long)record.Start,
                    EndTimeInMs = (long)record.End,
                    ParentType = record.Metadata["typeName"] as string,
                    Field = record.Metadata["fieldName"] as string,
                    Arguments = record.Metadata["arguments"] as Dictionary<string, object>,
                });
            }

            if (records.Any())
            {
                response.AddExtra("profile", records.OrderBy(record => record.Path));
            }
        }
    }
}
