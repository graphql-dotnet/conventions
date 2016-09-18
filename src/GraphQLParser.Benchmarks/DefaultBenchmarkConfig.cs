using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace GraphQLParser.Benchmarks {
    public sealed class DefaultBenchmarkConfig : ManualConfig {
        public DefaultBenchmarkConfig() {
            Add(Job.Default
                .With(Runtime.Clr)
                .With(Jit.RyuJit)
                .WithLaunchCount(3)
                .WithWarmupCount(5)
                .WithTargetCount(10)
            );

            #if NET45
            Add(new BenchmarkDotNet.Diagnostics.Windows.MemoryDiagnoser());
            #endif

            Add(Job.Default
                .With(Runtime.Core)
                .With(Jit.RyuJit)
                .WithLaunchCount(3)
                .WithWarmupCount(5)
                .WithTargetCount(10)
            );

            Add(StatisticColumn.AllStatistics);
        }
    }
}
