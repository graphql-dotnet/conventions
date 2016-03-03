using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Handlers;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Types.Descriptors
{
    public class GraphFieldInfo : GraphEntityInfo
    {
        private readonly ExecutionFilterAttributeHandler _executionFilterHandler =
            new ExecutionFilterAttributeHandler();

        private Func<IResolutionContext, object> _resolver;

        public GraphFieldInfo(ITypeResolver typeResolver, MemberInfo field = null)
            : base(typeResolver, field)
        {
            ResolveDelegate = null;
        }

        public GraphTypeInfo DeclaringType { get; set; }

        public object DefaultValue => Value ?? Type.DefaultValue;

        public List<GraphArgumentInfo> Arguments { get; } =
            new List<GraphArgumentInfo>();

        public List<IExecutionFilterAttribute> ExecutionFilters { get; } =
            new List<IExecutionFilterAttribute>();

        public Func<IResolutionContext, object> ResolveDelegate
        {
            get { return _resolver; }
            set
            {
                if (value == null)
                {
                    _resolver = _ => DefaultValue;
                    return;
                }

                _resolver = context =>
                {
                    return Task.Run(async () =>
                    {
                        var executionContext = await _executionFilterHandler.Execute(this, context, value).ConfigureAwait(false);
                        if (executionContext.DidSucceed)
                        {
                            return executionContext.Result;
                        }
                        else
                        {
                            throw new FieldResolutionException(this, context, executionContext.Exception);
                        }
                    }, context.CancellationToken);
                };
            }
        }
    }
}
