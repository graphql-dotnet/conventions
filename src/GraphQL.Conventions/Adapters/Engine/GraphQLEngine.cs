using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Conventions.Adapters.Engine.Utilities;
using GraphQL.Conventions.Builders;
using GraphQL.Conventions.Profiling;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Execution;
using GraphQL.Http;
using GraphQL.Types;
using GraphQL.Utilities;
using GraphQL.Validation;

namespace GraphQL.Conventions.Adapters.Engine
{
    public class GraphQLEngine
    {
        private readonly TypeResolver _typeResolver = new TypeResolver();

        private readonly GraphTypeAdapter _graphTypeAdapter = new GraphTypeAdapter();

        private readonly SchemaConstructor<ISchema, IGraphType> _constructor;

        private readonly DocumentExecuter _documentExecutor = new DocumentExecuter();

        private readonly IDocumentBuilder _documentBuilder = new GraphQLDocumentBuilder();

        private readonly DocumentValidator _documentValidator = new DocumentValidator();

        private readonly DocumentWriter _documentWriter = new DocumentWriter();

        private readonly SchemaPrinter _schemaPrinter;

        private readonly ISchema _schema;

        private class NoopValidationRule : IValidationRule
        {
            public INodeVisitor Validate(ValidationContext context)
            {
                return new EnterLeaveListener(_ => { });
            }
        }

        public GraphQLEngine(params Type[] schemaTypes)
            : this(null, schemaTypes)
        {
        }

        public GraphQLEngine(Func<Type, object> typeResolutionDelegate, params Type[] schemaTypes)
        {
            _constructor = new SchemaConstructor<ISchema, IGraphType>(_graphTypeAdapter, _typeResolver);
            _constructor.TypeResolutionDelegate = typeResolutionDelegate;
            _schema = _constructor.Build(schemaTypes);
            _schemaPrinter = new SchemaPrinter(_schema, new[] { TypeNames.Url, TypeNames.Uri, TypeNames.TimeSpan });
        }

        public string Describe()
        {
            return _schemaPrinter.Print();
        }

        public GraphQLExecutor NewExecutor(IRequestDeserializer requestDeserializer = null)
        {
            return new GraphQLExecutor(this, requestDeserializer ?? new RequestDeserializer());
        }

        internal IDependencyInjector DependencyInjector
        {
            get { return _typeResolver.DependencyInjector; }
            set { _typeResolver.DependencyInjector = value; }
        }

        internal void AddProfiler(IProfiler profiler)
        {
            _typeResolver.Profilers.Add(profiler);
        }

        internal async Task<ExecutionResult> Execute(
            object rootObject,
            string query,
            string operationName,
            Inputs inputs,
            object userContext,
            bool useValidation = true,
            IEnumerable<IValidationRule> rules = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!useValidation)
            {
                rules = new[] { new NoopValidationRule() };
            }
            return await _documentExecutor
                .ExecuteAsync(_schema, rootObject, query, operationName, inputs, userContext, cancellationToken, rules)
                .ConfigureAwait(false);
        }

        internal IValidationResult Validate(string queryString)
        {
            var document = _documentBuilder.Build(queryString);
            return _documentValidator.Validate(queryString, _schema, document);
        }

        internal string ConvertResultToString(ExecutionResult result)
        {
            return _documentWriter.Write(result);
        }
    }
}
