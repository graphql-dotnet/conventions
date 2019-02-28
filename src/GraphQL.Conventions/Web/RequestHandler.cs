using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Validation.Complexity;
using GraphQL.Conventions.Types.Resolution;

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
            readonly List<Type> _schemaTypes = new List<Type>();

            readonly List<Type> _assemblyTypes = new List<Type>();

            readonly List<Type> _exceptionsTreatedAsWarnings = new List<Type>();

            readonly List<Type> _middleware = new List<Type>();

            readonly TypeResolver _typeResolver = new TypeResolver();

            IDependencyInjector _dependencyInjector;

            ResolveTypeDelegate _resolveTypeDelegate;

            bool _useValidation = true;

            bool _useProfiling = false;

            bool _outputViolationsAsWarnings;

            FieldResolutionStrategy _fieldResolutionStrategy = FieldResolutionStrategy.Normal;

            ComplexityConfiguration _complexityConfiguration;

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
                _outputViolationsAsWarnings = outputViolationsAsWarnings;
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

            public RequestHandlerBuilder WithComplexityConfiguration(ComplexityConfiguration complexityConfiguration)
            {
                _complexityConfiguration = complexityConfiguration;
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
                    _outputViolationsAsWarnings,
                    _fieldResolutionStrategy,
                    _complexityConfiguration,
                    _middleware,
                    _typeResolver);
            }

            public object Resolve(TypeInfo typeInfo)
            {
                return _resolveTypeDelegate?.Invoke(typeInfo.AsType());
            }
        }

        class RequestHandlerImpl : IRequestHandler
        {
            readonly GraphQLEngine _engine;

            readonly IDependencyInjector _dependencyInjector;

            readonly List<Type> _exceptionsTreatedAsWarnings = new List<Type>();

            readonly bool _useValidation;

            readonly bool _useProfiling;

            readonly bool _outputViolationsAsWarnings;

            readonly ComplexityConfiguration _complexityConfiguration;

            internal RequestHandlerImpl(
                IDependencyInjector dependencyInjector,
                IEnumerable<Type> schemaTypes,
                IEnumerable<Type> assemblyTypes,
                IEnumerable<Type> exceptionsTreatedAsWarning,
                bool useValidation,
                bool useProfiling,
                bool outputViolationsAsWarnings,
                FieldResolutionStrategy fieldResolutionStrategy,
                ComplexityConfiguration complexityConfiguration,
                IEnumerable<Type> middleware,
                TypeResolver typeResolver)
            {
                _engine = new GraphQLEngine(typeResolver: typeResolver);
                _dependencyInjector = dependencyInjector;
                _engine.WithAttributesFromAssemblies(assemblyTypes);
                _exceptionsTreatedAsWarnings.AddRange(exceptionsTreatedAsWarning);
                _useValidation = useValidation;
                _useProfiling = useProfiling;
                _outputViolationsAsWarnings = outputViolationsAsWarnings;
                _engine.WithFieldResolutionStrategy(fieldResolutionStrategy);
                _engine.BuildSchema(schemaTypes.ToArray());
                _complexityConfiguration = complexityConfiguration;

                foreach (var type in middleware)
                {
                    _engine.WithMiddleware(type);
                }
            }

            public async Task<Response> ProcessRequest(Request request, IUserContext userContext, IDependencyInjector dependencyInjector = null)
            {
                var result = await _engine
                    .NewExecutor()
                    .WithQueryString(request.QueryString)
                    .WithInputs(request.Variables)
                    .WithOperationName(request.OperationName)
                    .WithDependencyInjector(dependencyInjector ?? _dependencyInjector)
                    .WithUserContext(userContext)
                    .WithComplexityConfiguration(_complexityConfiguration)
                    .EnableValidation(_useValidation)
                    .EnableProfiling(_useProfiling)
                    .Execute()
                    .ConfigureAwait(false);

                var response = new Response(request, result);
                var errors = result?.Errors?.Where(e => !string.IsNullOrWhiteSpace(e?.Message));
                foreach (var error in errors ?? new List<ExecutionError>())
                {
                    if (_exceptionsTreatedAsWarnings.Contains(error.InnerException.GetType()))
                    {
                        response.Warnings.Add(error);
                    }
                    else
                    {
                        response.Errors.Add(error);
                    }
                }
                result.Errors = new ExecutionErrors();
                result.Errors.AddRange(response.Errors);
                response.Body = _engine.SerializeResult(result);
                return response;
            }

            public Response Validate(Request request)
            {
                var result = _engine.Validate(request.QueryString);
                return new Response(request, result);
            }

            public string DescribeSchema(
                bool returnJson = false,
                bool includeFieldDescriptions = false,
                bool includeFieldDeprecationReasons = true)
            {
                if (returnJson)
                {
                    var result = _engine
                        .NewExecutor()
                        .WithQueryString(IntrospectionQuery)
                        .Execute()
                        .Result;
                    return _engine.SerializeResult(result);
                }
                _engine.PrintFieldDescriptions(includeFieldDescriptions);
                _engine.PrintFieldDeprecationReasons(includeFieldDeprecationReasons);
                return _engine.Describe();
            }

            #region Queries
            // Source: https://github.com/graphql/graphql-js/blob/master/src/utilities/introspectionQuery.js
            const string IntrospectionQuery = @"
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
