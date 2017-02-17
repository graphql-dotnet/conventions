using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Conventions.Web
{
    public static class RequestHandler
    {
        public static RequestHandlerBuilder New()
        {
            return new RequestHandlerBuilder();
        }

        public class RequestHandlerBuilder
        {
            readonly List<Type> _schemaTypes = new List<Type>();

            readonly List<Type> _assemblyTypes = new List<Type>();

            readonly List<Type> _exceptionsTreatedAsWarnings = new List<Type>();

            IDependencyInjector _dependencyInjector;

            internal RequestHandlerBuilder()
            {
            }

            public RequestHandlerBuilder WithDependencyInjector(IDependencyInjector dependencyInjector)
            {
                _dependencyInjector = dependencyInjector;
                return this;
            }

            public RequestHandlerBuilder WithQuery<TQuery>()
            {
                _schemaTypes.Add(typeof(SchemaDefinition<TQuery>));
                return this;
            }

            public RequestHandlerBuilder WithMutation<TMutation>()
            {
                _schemaTypes.Add(typeof(SchemaDefinitionWithMutation<TMutation>));
                return this;
            }

            public RequestHandlerBuilder WithQueryAndMutation<TQuery, TMutation>()
            {
                _schemaTypes.Add(typeof(SchemaDefinition<TQuery, TMutation>));
                return this;
            }

            public RequestHandlerBuilder WithSubscription<TSubscription>()
            {
                _schemaTypes.Add(typeof(SchemaDefinitionWithSubscription<TSubscription>));
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

            public IRequestHandler Generate()
            {
                return new RequestHandlerImpl(
                    _dependencyInjector,
                    _schemaTypes,
                    _assemblyTypes,
                    _exceptionsTreatedAsWarnings);
            }
        }

        class RequestHandlerImpl : IRequestHandler
        {
            readonly GraphQLEngine _engine = new GraphQLEngine();

            readonly IDependencyInjector _dependencyInjector;

            readonly List<Type> _exceptionsTreatedAsWarnings = new List<Type>();

            internal RequestHandlerImpl(
                IDependencyInjector dependencyInjector,
                IEnumerable<Type> schemaTypes,
                IEnumerable<Type> assemblyTypes,
                IEnumerable<Type> exceptionsTreatedAsWarning)
            {
                _dependencyInjector = dependencyInjector;
                _exceptionsTreatedAsWarnings.AddRange(exceptionsTreatedAsWarning);
                _engine.WithAttributesFromAssemblies(assemblyTypes);
                _engine.BuildSchema(schemaTypes.ToArray());
            }

            public async Task<Response> ProcessRequest(Request request, IUserContext userContext)
            {
                var result = await _engine
                    .NewExecutor()
                    .WithQueryString(request.QueryString)
                    .WithInputs(request.Variables)
                    .WithOperationName(request.OperationName)
                    .WithDependencyInjector(_dependencyInjector)
                    .WithUserContext(userContext)
                    .Execute()
                    .ConfigureAwait(false);

                var response = result.ToResponse(request, _engine);
                var errors = result.Errors?.Where(e => !string.IsNullOrWhiteSpace(e?.Message));

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

                return response;
            }

            public string DescribeSchema(bool returnJson = false)
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
                types {
                    ...FullType
                }
                directives {
                    name
                    description
                    args {
                    ...InputValue
                    }
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
                args {
                    ...InputValue
                }
                type {
                    ...TypeRef
                }
                isDeprecated
                deprecationReason
                }
                inputFields {
                ...InputValue
                }
                interfaces {
                ...TypeRef
                }
                enumValues(includeDeprecated: true) {
                name
                description
                isDeprecated
                deprecationReason
                }
                possibleTypes {
                ...TypeRef
                }
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
