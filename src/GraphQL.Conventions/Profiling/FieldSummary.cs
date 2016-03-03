using System.Collections.Concurrent;
using System.Collections.Generic;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Profiling
{
    public class FieldSummary
    {
        private readonly ConcurrentBag<long> _elapsedMilliseconds = new ConcurrentBag<long>();

        public ExecutionContext Context { get; set; }

        public GraphFieldInfo Field { get; set; }

        public string Identifier => $"{Field.DeclaringType.Name}.{Field.Name}";

        public IEnumerable<long> ElapsedMilliseconds => _elapsedMilliseconds.ToArray();

        public void AddResolutionStats(long elapsedMilliseconds)
        {
            _elapsedMilliseconds.Add(elapsedMilliseconds);
        }
    }
}
