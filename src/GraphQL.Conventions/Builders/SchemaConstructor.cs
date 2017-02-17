using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Builders
{
    public class SchemaConstructor<TSchemaType, TGraphType>
        where TSchemaType : class
    {
        private readonly IGraphTypeAdapter<TSchemaType, TGraphType> _graphTypeAdapter;

        private readonly ITypeResolver _typeResolver;

        private Func<Type, object> _typeResolutionDelegate;

        public SchemaConstructor(
            IGraphTypeAdapter<TSchemaType, TGraphType> graphTypeAdapter,
            ITypeResolver typeResolver = null)
        {
            _graphTypeAdapter = graphTypeAdapter;
            _typeResolver = typeResolver ?? new TypeResolver();
        }

        public Func<Type, object> TypeResolutionDelegate
        {
            get { return _typeResolutionDelegate ?? (type => Activator.CreateInstance(type)); }
            set { _typeResolutionDelegate = value; }
        }

        public TSchemaType Build<TSchema>() =>
            Build(typeof(TSchema).GetTypeInfo());

        public TSchemaType Build(params Type[] schemaTypes) =>
            Build(schemaTypes?.Select(type => type?.GetTypeInfo()).ToArray());

        public TSchemaType Build(params TypeInfo[] schemaTypes)
        {
            var schemaInfos = schemaTypes?
                .Select(_typeResolver.DeriveSchema)
                .ToList() ?? new List<GraphSchemaInfo>();

            var schemaInfo = schemaInfos.FirstOrDefault();
            if (schemaInfo == null)
            {
                return null;
            }
            else if (schemaInfos.Count == 1)
            {
                schemaInfo.TypeResolutionDelegate = TypeResolutionDelegate;
                return _graphTypeAdapter.DeriveSchema(schemaInfo);
            }

            schemaInfo = new GraphSchemaInfo
            {
                Query = _typeResolver.DeriveType(typeof(Query).GetTypeInfo()),
                Mutation = _typeResolver.DeriveType(typeof(Mutation).GetTypeInfo()),
                Subscription = _typeResolver.DeriveType(typeof(Subscription).GetTypeInfo()),
                TypeResolutionDelegate = TypeResolutionDelegate,
            };

            schemaInfo.Query.Name = nameof(Query);
            schemaInfo.Mutation.Name = nameof(Mutation);
            schemaInfo.Subscription.Name = nameof(Subscription);

            foreach (var additionalSchemaInfo in schemaInfos)
            {
                additionalSchemaInfo.TypeResolutionDelegate = TypeResolutionDelegate;

                schemaInfo.Query.Fields.AddRange(RemoveDuplicateViewers(
                    additionalSchemaInfo.Query?.Fields,
                    schemaInfo.Query.Fields));
                schemaInfo.Query.Fields = schemaInfo.Query.Fields.OrderBy(f => f.Name).ToList();
                UpdateTypeResolutionDelegate(schemaInfo, schemaInfo.Query.Fields);

                schemaInfo.Mutation.Fields.AddRange(RemoveDuplicateViewers(
                    additionalSchemaInfo.Mutation?.Fields,
                    schemaInfo.Mutation.Fields));
                schemaInfo.Mutation.Fields = schemaInfo.Mutation.Fields.OrderBy(f => f.Name).ToList();
                UpdateTypeResolutionDelegate(schemaInfo, schemaInfo.Mutation.Fields);

                schemaInfo.Subscription.Fields.AddRange(RemoveDuplicateViewers(
                    additionalSchemaInfo.Subscription?.Fields,
                    schemaInfo.Subscription.Fields));
                schemaInfo.Subscription.Fields = schemaInfo.Subscription.Fields.OrderBy(f => f.Name).ToList();
                UpdateTypeResolutionDelegate(schemaInfo, schemaInfo.Subscription.Fields);

                schemaInfo.Types.AddRange(additionalSchemaInfo.Types.Except(schemaInfo.Types));
            }

            if (!schemaInfo.Query.Fields.Any())
            {
                schemaInfo.Query = null;
            }
            if (!schemaInfo.Mutation.Fields.Any())
            {
                schemaInfo.Mutation = null;
            }
            if (!schemaInfo.Subscription.Fields.Any())
            {
                schemaInfo.Subscription = null;
            }

            return _graphTypeAdapter.DeriveSchema(schemaInfo);
        }

        private IEnumerable<GraphFieldInfo> RemoveDuplicateViewers(
            IEnumerable<GraphFieldInfo> fields,
            IEnumerable<GraphFieldInfo> existingFields)
        {
            var registeredViewerClasses = new[]
            {
                typeof(ImplementViewerAttribute.QueryViewerReferrer),
                typeof(ImplementViewerAttribute.MutationViewerReferrer),
                typeof(ImplementViewerAttribute.SubscriptionViewerReferrer),
            };

            foreach (var field in fields ?? new List<GraphFieldInfo>())
            {
                if (existingFields.Any(existingField => existingField.Name == "viewer") &&
                    field.Name == "viewer" &&
                    registeredViewerClasses.Contains(field.DeclaringType.TypeRepresentation.AsType()))
                {
                    continue;
                }
                yield return field;
            }
        }

        private void UpdateTypeResolutionDelegate(
            GraphSchemaInfo schema,
            IEnumerable<GraphFieldInfo> fields)
        {
            foreach (var field in fields)
            {
                field.SchemaInfo = schema;
            }
        }

        private class Query { }

        private class Mutation { }

        private class Subscription { }
    }
}
