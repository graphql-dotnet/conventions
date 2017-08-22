using System.Collections.Generic;
using Newtonsoft.Json;

namespace GraphQL.Conventions.Extensions
{
    public class PerformanceRecord
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "start")]
        public long StartTimeInMs { get; set; }

        [JsonProperty(PropertyName = "end")]
        public long EndTimeInMs { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string ParentType { get; set; }

        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "args")]
        public Dictionary<string, object> Arguments { get; set; }
    }
}
