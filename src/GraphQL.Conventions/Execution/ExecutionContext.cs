using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Profiling;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Execution
{
    public class ExecutionContext
    {
        private Exception _exception;

        internal ExecutionContext(GraphEntityInfo entity, IResolutionContext resolutionContext)
        {
            Entity = entity;
            ResolutionContext = resolutionContext;
        }

        public IResolutionContext ResolutionContext { get; private set; }

        public object RootContext { get; private set; }

        public List<IProfiler> Profilers => Entity.TypeResolver.Profilers;

        public bool IsProfilable => Entity.IsProfilable && Profilers.Any();

        public GraphEntityInfo Entity { get; private set; }

        public object Result { get; set; }

        public bool DidExecute { get; internal set; }

        public bool DidSucceed => DidExecute && Exception == null;

        public Exception Exception
        {
            get { return _exception; }
            internal set
            {
                Exception innerException;
                _exception = value;
                while ((innerException = ExtractInnerException(_exception)) != null)
                {
                    _exception = innerException;
                }
            }
        }

        private Exception ExtractInnerException(Exception exception)
        {
            if (exception is TargetInvocationException)
            {
                return exception.InnerException;
            }
            else if (exception is AggregateException &&
                ((AggregateException)exception).InnerExceptions.Count == 1)
            {
                return exception.InnerException;
            }
            return null;
        }
    }
}
