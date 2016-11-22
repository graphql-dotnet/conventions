using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Conventions.Adapters.Engine.Listeners.DataLoader;
using GraphQL.Conventions.Adapters.Engine.Utilities;
using GraphQL.Conventions.Builders;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Execution;
using GraphQL.Http;
using GraphQL.Instrumentation;
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

        private SchemaPrinter _schemaPrinter;

        private ISchema _schema;

        private class NoopValidationRule : IValidationRule
        {
            public INodeVisitor Validate(ValidationContext context)
            {
                return new EnterLeaveListener(_ => { });
            }
        }

        public GraphQLEngine(Func<System.Type, object> typeResolutionDelegate = null)
        {
            _constructor = new SchemaConstructor<ISchema, IGraphType>(_graphTypeAdapter, _typeResolver);
            _constructor.TypeResolutionDelegate = typeResolutionDelegate;
        }

        public void BuildSchema(params System.Type[] schemaTypes)
        {
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

        public void RegisterScalarType<TType, TGraphType>(string name = null)
        {
            _typeResolver.RegisterScalarType<TType>(name ?? typeof(TType).Name);
            _graphTypeAdapter.RegisterScalarType<TGraphType>(name ?? typeof(TType).Name);
        }

        public string SerializeResult(ExecutionResult result)
        {
            return _documentWriter.Write(result);
        }

        internal IDependencyInjector DependencyInjector
        {
            get { return _typeResolver.DependencyInjector; }
            set { _typeResolver.DependencyInjector = value; }
        }

        internal async Task<ExecutionResult> Execute(
            object rootObject,
            string query,
            string operationName,
            Inputs inputs,
            IUserContext userContext,
            bool enableValidation = true,
            bool enableProfiling = false,
            IEnumerable<IValidationRule> rules = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!enableValidation)
            {
                rules = new[] { new NoopValidationRule() };
            }
            var configuration = new ExecutionOptions
            {
                Schema = _schema,
                Root = rootObject,
                Query = query,
                OperationName = operationName,
                Inputs = inputs,
                UserContext = userContext,
                ValidationRules = rules,
                CancellationToken = cancellationToken,
            };

            if (userContext is IDataLoaderContextProvider)
            {
                configuration.Listeners.Add(new DataLoaderListener());
            }

            if (enableProfiling)
            {
                configuration.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
            }

            var result = await _documentExecutor.ExecuteAsync(configuration).ConfigureAwait(false);

            if (result.Errors != null)
            {
                var errors = new ExecutionErrors();
                foreach (var executionError in result.Errors)
                {
                    var exception = new FieldResolutionException(executionError);
                    var error = new ExecutionError(exception.Message, exception);
                    foreach (var location in executionError.Locations ?? new ErrorLocation[0])
                    {
                        error.AddLocation(location.Line, location.Column);
                    }
                    errors.Add(error);
                }
                result.Errors = errors;
            }

            return result;
        }

        internal IValidationResult Validate(string queryString)
        {
            var document = _documentBuilder.Build(queryString);
            return _documentValidator.Validate(queryString, _schema, document);
        }
    }
}
