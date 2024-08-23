using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Adapters.Engine.ErrorTransformations;
using GraphQL.Conventions.Adapters.Engine.Listeners.DataLoader;
using GraphQL.Conventions.Builders;
using GraphQL.Conventions.Extensions;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQL.NewtonsoftJson;
using GraphQL.Types;
using GraphQL.Utilities;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using GraphQL.Validation.Rules.Custom;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public class GraphQLEngine
    {
        private readonly ITypeResolver _typeResolver = new TypeResolver();

        private readonly GraphTypeAdapter _graphTypeAdapter = new GraphTypeAdapter();

        private readonly SchemaConstructor<ISchema, IGraphType> _constructor;

        private readonly IDocumentExecuter _documentExecutor;

        private readonly IDocumentBuilder _documentBuilder = new GraphQLDocumentBuilder();

        private readonly DocumentValidator _documentValidator = new DocumentValidator();

        private readonly GraphQLSerializer _documentSerializer = new GraphQLSerializer();

        private PrintOptions _printOptions;

        private ISchema _schema;

        private readonly object _schemaLock = new object();

        private readonly List<Type> _schemaTypes = new List<Type>();

        private readonly List<Type> _middleware = new List<Type>();

        private IErrorTransformation _errorTransformation = new DefaultErrorTransformation();

        private bool _includeFieldDescriptions;
        private bool _throwUnhandledExceptions;

        private bool _includeFieldDeprecationReasons;

        private class NoopValidationRule : ValidationRuleBase
            {
            }

        private class WrappedDependencyInjector : IDependencyInjector
        {
            private readonly Func<Type, object> _typeResolutionDelegate;

            public WrappedDependencyInjector(Func<Type, object> typeResolutionDelegate)
            {
                _typeResolutionDelegate = typeResolutionDelegate;
            }

            public object Resolve(System.Reflection.TypeInfo typeInfo)
            {
                return _typeResolutionDelegate(typeInfo.AsType());
            }
        }

        public GraphQLEngine(Func<Type, object> typeResolutionDelegate = null, ITypeResolver typeResolver = null, IDocumentExecuter documentExecutor = null)
        {
            _documentExecutor = documentExecutor ?? new GraphQL.DocumentExecuter();
            _typeResolver = typeResolver ?? _typeResolver;
            _constructor = new SchemaConstructor<ISchema, IGraphType>(_graphTypeAdapter, _typeResolver)
            {
                TypeResolutionDelegate = typeResolutionDelegate != null
                    ? type => typeResolutionDelegate(type) ?? CreateInstance(type)
                    : (Func<Type, object>)CreateInstance
            };
        }

        public static GraphQLEngine New(IDocumentExecuter documentExecutor)
            => new GraphQLEngine(null, null, documentExecutor);

        public static GraphQLEngine New(Func<Type, object> typeResolutionDelegate = null)
        {
            return new GraphQLEngine(typeResolutionDelegate);
        }

        public static GraphQLEngine New<TQuery>(Func<Type, object> typeResolutionDelegate = null)
        {
            return New(typeResolutionDelegate)
                .WithQuery<TQuery>()
                .BuildSchema();
        }

        public static GraphQLEngine New<TQuery, TMutation>(Func<Type, object> typeResolutionDelegate = null)
        {
            return New(typeResolutionDelegate)
                .WithQueryAndMutation<TQuery, TMutation>()
                .BuildSchema();
        }

        public GraphQLEngine WithFieldResolutionStrategy(FieldResolutionStrategy strategy)
        {
            switch (strategy)
            {
                default:
                    _graphTypeAdapter.FieldResolverFactory = fieldInfo => new FieldResolver(fieldInfo);
                    break;

                case FieldResolutionStrategy.WrappedAsynchronous:
                    _graphTypeAdapter.FieldResolverFactory = fieldInfo => new WrappedAsyncFieldResolver(fieldInfo);
                    break;

                case FieldResolutionStrategy.WrappedSynchronous:
                    _graphTypeAdapter.FieldResolverFactory = fieldInfo => new WrappedSyncFieldResolver(fieldInfo);
                    break;
            }
            return this;
        }

        public GraphQLEngine WithQuery<TQuery>()
        {
            _schemaTypes.Add(typeof(SchemaDefinition<TQuery>));
            return this;
        }

        public GraphQLEngine WithMutation<TMutation>()
        {
            _schemaTypes.Add(typeof(SchemaDefinitionWithMutation<TMutation>));
            return this;
        }

        public GraphQLEngine WithQueryAndMutation<TQuery, TMutation>()
        {
            _schemaTypes.Add(typeof(SchemaDefinition<TQuery, TMutation>));
            return this;
        }

        public GraphQLEngine WithSubscription<TSubscription>()
        {
            _schemaTypes.Add(typeof(SchemaDefinitionWithSubscription<TSubscription>));
            return this;
        }

        public GraphQLEngine WithAttributesFromAssembly(Type assemblyType)
        {
            _typeResolver.RegisterAttributesInAssembly(assemblyType);
            return this;
        }

        public GraphQLEngine WithAttributesFromAssembly<TAssemblyType>()
        {
            return WithAttributesFromAssembly(typeof(TAssemblyType));
        }

        public GraphQLEngine WithAttributesFromAssemblies(IEnumerable<Type> assemblyTypes)
        {
            foreach (var assemblyType in assemblyTypes)
            {
                WithAttributesFromAssembly(assemblyType);
            }
            return this;
        }

        public GraphQLEngine WithMiddleware(Type type)
        {
            _middleware.Add(type);
            return this;
        }

        public GraphQLEngine WithMiddleware<T>()
        {
            return WithMiddleware(typeof(T));
        }

        public GraphQLEngine WithCustomErrorTransformation(IErrorTransformation errorTransformation)
        {
            _errorTransformation = errorTransformation;
            return this;
        }

        public GraphQLEngine WithQueryExtensions(Type typeExtensions)
        {
            _typeResolver.AddExtensions(typeExtensions);
            return this;
        }

        public GraphQLEngine PrintFieldDescriptions(bool include = true)
        {
            _includeFieldDescriptions = include;
            return this;
        }

        public GraphQLEngine ThrowUnhandledExceptions(bool throwUnhandledExceptions = true)
        {
            _throwUnhandledExceptions = throwUnhandledExceptions;
            return this;
        }

        public GraphQLEngine PrintFieldDeprecationReasons(bool include = true)
        {
            _includeFieldDeprecationReasons = include;
            return this;
        }

        public GraphQLEngine BuildSchema(params Type[] types)
        {
            return BuildSchema(null, types);
        }

        public GraphQLEngine BuildSchema(PrintOptions options, params Type[] types)
        {
            if (_schema != null)
                return this;
            if (types.Length > 0)
            {
                _schemaTypes.AddRange(types);
            }
            lock (_schemaLock)
                _schema = _constructor.Build(_schemaTypes.ToArray());
            _printOptions = options ?? new PrintOptions()
            {
                IncludeDescriptions = _includeFieldDescriptions,
                IncludeDeprecationReasons = _includeFieldDeprecationReasons,
                StringComparison = StringComparison.InvariantCultureIgnoreCase,
            };
            return this;
        }

        public string Describe(PrintOptions printOptions = null)
        {
            BuildSchema(); // Ensure that the schema has been constructed
            return _schema.Print(_printOptions = printOptions ?? _printOptions);
        }

        public ISchema GetSchema()
        {
            BuildSchema(); // Ensure that the schema has been constructed
            return _schema;
        }

        public GraphQLExecutor NewExecutor(IRequestDeserializer requestDeserializer = null)
        {
            BuildSchema(); // Ensure that the schema has been constructed
            return new GraphQLExecutor(this, requestDeserializer ?? new RequestDeserializer());
        }

        public GraphQLEngine RegisterScalarType<TType, TGraphType>(string name = null)
        {
            _typeResolver.RegisterScalarType<TType>(name ?? typeof(TType).Name);
            _graphTypeAdapter.RegisterScalarType<TGraphType>(name ?? typeof(TType).Name);
            return this;
        }

        public string SerializeResult(ExecutionResult result) => _documentSerializer.Serialize(result);

        internal async Task<ExecutionResult> ExecuteAsync(
            object rootObject,
            string query,
            string operationName,
            Inputs variables,
            IUserContext userContext,
            IDependencyInjector dependencyInjector,
            ComplexityConfiguration complexityConfiguration,
            bool enableValidation = true,
            bool enableProfiling = false,
            IEnumerable<IValidationRule> rules = null,
            CancellationToken cancellationToken = default,
            IEnumerable<IDocumentExecutionListener> listeners = null)
        {
            dependencyInjector ??= new WrappedDependencyInjector(_constructor.TypeResolutionDelegate);

            if (!enableValidation)
            {
                rules = new[] { new NoopValidationRule() };
            }

            if (rules == null || rules.Count() == 0)
            {
                rules = DocumentValidator.CoreRules;
            }

            if (complexityConfiguration != null)
            {
                rules = rules.Append(new ComplexityValidationRule(complexityConfiguration));
            }

            var configuration = new ExecutionOptions
            {
                Schema = _schema,
                Root = rootObject,
                Query = query,
                OperationName = operationName,
                Variables = variables,
                EnableMetrics = enableProfiling,
                UserContext = new Dictionary<string, object>()
                {
                    { typeof(IUserContext).FullName ?? nameof(IUserContext), userContext },
                    { typeof(IDependencyInjector).FullName ?? nameof(IDependencyInjector), dependencyInjector },
                },
                ValidationRules = rules,
                CancellationToken = cancellationToken,
                ThrowOnUnhandledException = _throwUnhandledExceptions,
            };

            if (listeners != null)
            {
                foreach (var listener in listeners)
                    configuration.Listeners.Add(listener);
            }

            if (userContext is IDataLoaderContextProvider)
            {
                configuration.Listeners.Add(new DataLoaderListener());
            }

            if (enableProfiling)
            {
                _schema.FieldMiddleware.Use(dependencyInjector.Resolve<InstrumentFieldsMiddleware>());
            }

            foreach (var middleware in _middleware)
            {
                _schema.FieldMiddleware.Use(dependencyInjector.Resolve(middleware.GetTypeInfo()) as IFieldMiddleware);
            }

            var result = await _documentExecutor.ExecuteAsync(configuration).ConfigureAwait(false);

            if (result.Errors != null && _errorTransformation != null)
            {
                result.Errors = _errorTransformation.Transform(result.Errors);
            }

            return result;
        }

        internal async Task<IValidationResult> ValidateAsync(string queryString)
        {
            var document = _documentBuilder.Build(queryString);
            var result = await _documentValidator.ValidateAsync(new ValidationOptions()
            {
                Schema = _schema,
                Document = document
            });

            return result;
        }

        private object CreateInstance(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsAbstract && !typeInfo.ContainsGenericParameters)
            {
                var ctors = typeInfo.GetConstructors().ToList();
                if (ctors.All(ctor => ctor.GetParameters().Any()))
                {
                    var ctor = ctors.FirstOrDefault();
                    if (ctor == null)
                    {
                        return null;
                    }
                    var parameters = ctor.GetParameters();
                    var parameterValues = parameters
                        .Select(parameter => _constructor.TypeResolutionDelegate(parameter.ParameterType))
                        .ToArray();
                    return ctor.InvokeEnhanced(parameterValues);
                }
            }

            return Activator.CreateInstance(type);
        }
    }
}
