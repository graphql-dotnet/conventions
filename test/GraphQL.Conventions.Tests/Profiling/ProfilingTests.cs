using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Conventions.Adapters.Engine;
using GraphQL.Conventions.Profiling;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Descriptors;
using Xunit;

namespace GraphQL.Conventions.Tests.Profiling
{
    public class ProfilingTests : TestBase
    {
        [Fact]
        public async void Can_Collect_Profiling_Information()
        {
            var profiler = new Profiler();
            var result = await ExecuteQuery(@"{ field { a b } }", profiler);
            result.ShouldHaveNoErrors();

            profiler.Count().ShouldEqual(3);

            var field = profiler["Query.field"];
            field.ElapsedMilliseconds.Count().ShouldEqual(1);
            Assert.True(field.ElapsedMilliseconds.All(dur => dur >= 50));

            var a = profiler["SomeObject.a"];
            a.ElapsedMilliseconds.Count().ShouldEqual(3);
            Assert.True(a.ElapsedMilliseconds.All(dur => dur >= 1));

            var b = profiler["SomeObject.b"];
            b.ElapsedMilliseconds.Count().ShouldEqual(3);
            Assert.True(b.ElapsedMilliseconds.All(dur => dur >= 0));
        }

        private async Task<ExecutionResult> ExecuteQuery(string query, IProfiler profiler)
        {
            var engine = new GraphQLEngine();
            engine.BuildSchema(typeof(SchemaDefinition<Query>));
            var result = await engine
                .NewExecutor()
                .WithProfiler(profiler)
                .WithQueryString(query)
                .Execute();
            return result;
        }

        class Profiler : ProfilerBase
        {
            private ConcurrentDictionary<GraphFieldInfo, FieldSummary> _stats = new ConcurrentDictionary<GraphFieldInfo, FieldSummary>();

            public override void SummarizeField(FieldSummary summary, long correlationId)
            {
                _stats.AddOrUpdate(summary.Field, summary, (key, oldValue) => summary);
            }

            public FieldSummary this[string key]
            {
                get { return _stats.Values.FirstOrDefault(s => s.Identifier == key); }
            }

            public int Count() => _stats.Values.Count;
        }

        class Query
        {
            public async Task<IEnumerable<SomeObject>> Field()
            {
                var result = new List<SomeObject>
                {
                    new SomeObject(1),
                    new SomeObject(2),
                    new SomeObject(3),
                };
                await Task.Delay(100);
                return result;
            }
        }

        class SomeObject
        {
            private readonly int _value;

            public SomeObject(int value)
            {
                _value = value;
            }

            public async Task<bool> A()
            {
                await Task.Delay(10);
                return true;
            }

            public async Task<int> B()
            {
                await Task.Delay(1);
                return _value;
            }
        }
    }
}
