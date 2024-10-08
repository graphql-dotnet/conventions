using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Instrumentation;
using GraphQL.Validation.Complexity;
using Type = System.Type;

namespace GraphQL.Conventions.Web
{
    public static class RequestHandler
    {
        public delegate object ResolveTypeDelegate(Type type);

        public static RequestHandlerBuilder New()
        {
            return new RequestHandlerBuilder();
        }

        public class RequestHandlerBuilder : IDependencyInjector
        {
            private readonly List<Type> _schemaTypes = new List<Type>();
            private readonly List<Type> _assemblyTypes = new List<Type>();
            private readonly List<Type> _exceptionsTreatedAsWarnings = new List<Type>();
            private readonly List<Type> _middleware = new List<Type>();
            private readonly ITypeResolver _typeResolver = new TypeResolver();
            private IDependencyInjector _dependencyInjector;
            private ResolveTypeDelegate _resolveTypeDelegate;
            private bool _useValidation = true;
            private bool _useProfiling;
            private FieldResolutionStrategy _fieldResolutionStrategy = FieldResolutionStrategy.Normal;
            private LegacyComplexityConfiguration _complexityConfiguration;
            private ComplexityOptions _complexityOptions;

            internal RequestHandlerBuilder()
            {
                _dependencyInjector = this;
            }

            public RequestHandlerBuilder WithDependencyInjector(IDependencyInjector dependencyInjector)
            {
                _dependencyInjector = dependencyInjector;
                return this;
            }

            public RequestHandlerBuilder WithDependencyInjector(ResolveTypeDelegate resolveTypeDelegate)
            {
                _resolveTypeDelegate = resolveTypeDelegate;
                return this;
            }

            public RequestHandlerBuilder WithQuery<TQuery>()
            {
                _schemaTypes.Add(typeof(SchemaDefinition<TQuery>));
                return this;
            }

            public RequestHandlerBuilder WithQuery(Type type)
            {
                _schemaTypes.Add(typeof(SchemaDefinition<>).MakeGenericType(type));
                return this;
            }

            public RequestHandlerBuilder WithMutation<TMutation>()
            {
                _schemaTypes.Add(typeof(SchemaDefinitionWithMutation<TMutation>));
                return this;
            }

            public RequestHandlerBuilder WithMutation(Type type)
            {
                _schemaTypes.Add(typeof(SchemaDefinitionWithMutation<>).MakeGenericType(type));
                return this;
            }

            public RequestHandlerBuilder WithQueryAndMutation<TQuery, TMutation>()
            {
                _schemaTypes.Add(typeof(SchemaDefinition<TQuery, TMutation>));
                return this;
            }

            public RequestHandlerBuilder WithQueryExtensions(Type typeExtensions)
            {
                _typeResolver.AddExtensions(typeExtensions);
                return this;
            }

            public RequestHandlerBuilder WithSubscription<TSubscription>()
            {
                _schemaTypes.Add(typeof(SchemaDefinitionWithSubscription<TSubscription>));
                return this;
            }

            public RequestHandlerBuilder WithSubscription(Type type)
            {
                _schemaTypes.Add(typeof(SchemaDefinitionWithSubscription<>).MakeGenericType(type));
                return this;
            }

            public RequestHandlerBuilder WithAttributesFromAssembly<TAssemblyType>()
            {
                return WithAttributesFromAssembly(typeof(TAssemblyType));
            }

            public RequestHandlerBuilder WithAttributesFromAssembly(Type assemblyType)
            {
                _assemblyTypes.Add(assemblyType);
                return this;
            }

            public RequestHandlerBuilder WithAttributesFromAssemblies(IEnumerable<Type> assemblyTypes)
            {
                _assemblyTypes.AddRange(assemblyTypes);
                return this;
            }

            public RequestHandlerBuilder TreatAsWarning<TException>()
            {
                _exceptionsTreatedAsWarnings.Add(typeof(TException));
                return this;
            }

            public RequestHandlerBuilder WithoutValidation(bool outputViolationsAsWarnings = false)
            {
                _useValidation = false;
                return this;
            }

            public RequestHandlerBuilder WithProfiling(bool profiling = true)
            {
                _useProfiling = profiling;
                return this;
            }

            public RequestHandlerBuilder WithFieldResolutionStrategy(FieldResolutionStrategy strategy)
            {
                _fieldResolutionStrategy = strategy;
                return this;
            }

            [Obsolete("Please use the WithComplexityOptions method instead.")]
            public RequestHandlerBuilder WithComplexityConfiguration(LegacyComplexityConfiguration complexityConfiguration)
            {
                _complexityConfiguration = complexityConfiguration;
                return this;
            }

            public RequestHandlerBuilder WithComplexityOptions(ComplexityOptions complexityOptions)
            {
                _complexityOptions = complexityOptions;
                return this;
            }

            public RequestHandlerBuilder WithMiddleware<T>()
            {
                _middleware.Add(typeof(T));
                return this;
            }

            public RequestHandlerBuilder IgnoreTypesFromNamespacesStartingWith(params string[] namespacesToIgnore)
            {
                _typeResolver.IgnoreTypesFromNamespacesStartingWith(namespacesToIgnore);
                return this;
            }

            public IRequestHandler Generate()
            {
                return new RequestHandlerImpl(
                    _dependencyInjector,
                    _schemaTypes,
                    _assemblyTypes,
                    _exceptionsTreatedAsWarnings,
                    _useValidation,
                    _useProfiling,
                    _fieldResolutionStrategy,
                    _complexityConfiguration,
                    _complexityOptions,
                    _middleware,
                    _typeResolver);
            }

            public object Resolve(TypeInfo typeInfo)
            {
                return _resolveTypeDelegate?.Invoke(typeInfo.AsType());
            }
        }

        private class RequestHandlerImpl : IRequestHandler
        {
            private readonly GraphQLEngine _engine;
            private readonly IDependencyInjector _dependencyInjector;
            private readonly List<Type> _exceptionsTreatedAsWarnings = new List<Type>();
            private readonly bool _useValidation;
            private readonly bool _useProfiling;
            private readonly LegacyComplexityConfiguration _complexityConfiguration;
            private readonly ComplexityOptions _complexityOptions;

            internal RequestHandlerImpl(
                IDependencyInjector dependencyInjector,
                IEnumerable<Type> schemaTypes,
                IEnumerable<Type> assemblyTypes,
                IEnumerable<Type> exceptionsTreatedAsWarning,
                bool useValidation,
                bool useProfiling,
                FieldResolutionStrategy fieldResolutionStrategy,
                LegacyComplexityConfiguration complexityConfiguration,
                ComplexityOptions complexityOptions,
                IEnumerable<Type> middleware,
                ITypeResolver typeResolver)
            {
                _engine = new GraphQLEngine(typeResolver: typeResolver);
                _dependencyInjector = dependencyInjector;
                _engine.WithAttributesFromAssemblies(assemblyTypes);
                _exceptionsTreatedAsWarnings.AddRange(exceptionsTreatedAsWarning);
                _useValidation = useValidation;
                _useProfiling = useProfiling;
                _engine.WithFieldResolutionStrategy(fieldResolutionStrategy);
                _engine.BuildSchema(schemaTypes.ToArray());
                _complexityConfiguration = complexityConfiguration;
                _complexityOptions = complexityOptions;

                foreach (var type in middleware)
                {
                    _engine.WithMiddleware(type);
                }
            }

            public async Task<Response> ProcessRequestAsync(Request request, IUserContext userContext, IDependencyInjector dependencyInjector = null)
            {
                var start = DateTime.UtcNow;

                var result = await _engine
                    .NewExecutor()
                    .WithQueryString(request.QueryString)
                    .WithVariables(request.Variables)
                    .WithOperationName(request.OperationName)
                    .WithDependencyInjector(dependencyInjector ?? _dependencyInjector)
                    .WithUserContext(userContext)
                    .WithComplexityConfiguration(_complexityConfiguration)
                    .WithComplexityOptions(_complexityOptions)
                    .EnableValidation(_useValidation)
                    .EnableProfiling(_useProfiling)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                if (_useProfiling)
                {
                    result.EnrichWithApolloTracing(start);
                }

                var response = new Response(request, result);
                var errors = result?.Errors?.Where(e => !string.IsNullOrWhiteSpace(e?.Message));
                foreach (var error in errors ?? new List<ExecutionError>())
                {
                    if (_exceptionsTreatedAsWarnings.Contains(error.InnerException?.GetType()))
                    {
                        response.Warnings.Add(error);
                    }
                    else
                    {
                        response.Errors.Add(error);
                    }
                }

                if (result == null)
                    return response;
                result.Errors = new ExecutionErrors();
                result.Errors.AddRange(response.Errors);
                response.SetBody(_engine.SerializeResult(result));

                return response;
            }

            public async Task<Response> ValidateAsync(Request request)
            {
                var result = await _engine.ValidateAsync(request.QueryString);
                return new Response(request, result);
            }

            public async Task<string> DescribeSchemaAsync(
                bool returnJson = false,
                bool includeFieldDescriptions = false,
                bool includeFieldDeprecationReasons = true)
            {
                if (returnJson)
                {
                    var result = await _engine
                        .NewExecutor()
                        .WithQueryString(IntrospectionQuery)
                        .ExecuteAsync();

                    return _engine.SerializeResult(result);
                }

                _engine.PrintFieldDescriptions(includeFieldDescriptions);
                _engine.PrintFieldDeprecationReasons(includeFieldDeprecationReasons);
                return _engine.Describe();
            }

            #region Queries
            // Source: https://github.com/graphql/graphql-js/blob/master/src/utilities/introspectionQuery.js
            private const string IntrospectionQuery = @"
            query IntrospectionQuery {
                __schema {
                    queryType { name }
                    mutationType { name }
                    subscriptionType { name }
                    types { ...FullType }
                    directives {
                        name
                        description
                        args { ...InputValue }
                        onOperation
                        onFragment
                        onField
                    }
                }
            }
            fragment FullType on __Type {
                kind
                name
                description
                fields(includeDeprecated: true) {
                    name
                    description
                    args { ...InputValue }
                    type { ...TypeRef }
                    isDeprecated
                    deprecationReason
                }
                inputFields { ...InputValue }
                interfaces { ...TypeRef }
                enumValues(includeDeprecated: true) {
                    name
                    description
                    isDeprecated
                    deprecationReason
                }
                possibleTypes { ...TypeRef }
            }
            fragment InputValue on __InputValue {
                name
                description
                type { ...TypeRef }
                defaultValue
            }
            fragment TypeRef on __Type {
                kind
                name
                ofType {
                    kind
                    name
                    ofType {
                        kind
                        name
                        ofType {
                            kind
                            name
                        }
                    }
                }
            }
            ";
            #endregion
        }
    }
}
