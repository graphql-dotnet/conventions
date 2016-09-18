using System.Reflection;
using BenchmarkDotNet.Running;

namespace GraphQLParser.Benchmarks {
    internal static class Program {
        private static void Main() {
            var benchmarksAssembly = typeof(Program).GetTypeInfo().Assembly;
            var benchmarkSwitcher = new BenchmarkSwitcher(benchmarksAssembly);
            benchmarkSwitcher.Run();
        }
    }
}
