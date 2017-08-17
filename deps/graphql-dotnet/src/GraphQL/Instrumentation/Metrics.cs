using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraphQL.Instrumentation
{
    public class Metrics : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly IList<PerfRecord> _records = new List<PerfRecord>();
        private PerfRecord _main;
        private object _lock = new object();

        public void Start(string operationName)
        {
            _main = new PerfRecord("operation", operationName, 0);
            lock (_lock)
            {
                _records.Add(_main);
            }
            _stopwatch.Start();
        }

        public void SetOperationName(string name)
        {
            _main.Subject = name;
        }

        public IDisposable Subject(string category, string subject, Dictionary<string, object> metadata = null)
        {
            var record = new PerfRecord(category, subject, _stopwatch.ElapsedMilliseconds, metadata);
            lock (_lock)
            {
                _records.Add(record);
            }
            return new Marker(record, _stopwatch);
        }

        public IEnumerable<PerfRecord> AllRecords
        {
            get
            {
                lock (_lock)
                {
                    var allRecords = _records.OrderBy(x => x.Start).ToArray();
                    return allRecords;
                }
            }
        }

        public IEnumerable<PerfRecord> Finish()
        {
            _main?.MarkEnd(_stopwatch.ElapsedMilliseconds);
            _stopwatch.Stop();
            return AllRecords;
        }

        public class Marker : IDisposable
        {
            private readonly PerfRecord _record;
            private readonly Stopwatch _stopwatch;

            public Marker(PerfRecord record, Stopwatch stopwatch)
            {
                _record = record;
                _stopwatch = stopwatch;
            }

            public void Dispose()
            {
                _record.MarkEnd(_stopwatch.ElapsedMilliseconds);
            }
        }

        public void Dispose()
        {
            if (_stopwatch.IsRunning) _stopwatch.Stop();
        }
    }
}
