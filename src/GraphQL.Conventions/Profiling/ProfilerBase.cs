using System.Collections.Concurrent;
using System.Diagnostics;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Profiling
{
    public abstract class ProfilerBase : IProfiler
    {
        private readonly ConcurrentDictionary<long, Stopwatch> _fieldStopwatches =
            new ConcurrentDictionary<long, Stopwatch>();

        private readonly ConcurrentDictionary<GraphFieldInfo, FieldSummary> _fieldSummaries =
            new ConcurrentDictionary<GraphFieldInfo, FieldSummary>();

        public virtual void EnterResolver(ExecutionContext context, long correlationId)
        {
            var stopwatch = new Stopwatch();
            _fieldStopwatches.AddOrUpdate(correlationId, stopwatch, (index, old) => stopwatch);
            stopwatch.Start();
        }

        public virtual void ExitResolver(ExecutionContext context, long correlationId)
        {
            Stopwatch stopwatch;
            if (!_fieldStopwatches.TryGetValue(correlationId, out stopwatch))
            {
                return;
            }

            stopwatch.Stop();

            var fieldInfo = context.Entity as GraphFieldInfo;
            var fieldSummary =  new FieldSummary
            {
                Context = context,
                Field = fieldInfo,
            };
            fieldSummary.AddResolutionStats(stopwatch.ElapsedMilliseconds);

            fieldSummary = _fieldSummaries.AddOrUpdate(fieldSummary.Field, fieldSummary, (key, oldValue) =>
            {
                oldValue.AddResolutionStats(stopwatch.ElapsedMilliseconds);
                return oldValue;
            });

            SummarizeField(fieldSummary, correlationId);
        }

        public abstract void SummarizeField(FieldSummary summary, long correlationId);
    }
}
