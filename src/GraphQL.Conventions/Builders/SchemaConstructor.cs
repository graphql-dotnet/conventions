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
            Build(schemaTypes.Select(type => type.GetTypeInfo()).ToArray());

        public TSchemaType Build(params TypeInfo[] schemaTypes)
        {
            var schemaInfos = schemaTypes
                .Select(_typeResolver.DeriveSchema)
                .ToList();

            var schemaInfo = schemaInfos.FirstOrDefault();
            schemaInfo.TypeResolutionDelegate = TypeResolutionDelegate;

            foreach (var additionalSchemaInfo in schemaInfos.Skip(1))
            {
                if (schemaInfo.Query != null)
                {
                    schemaInfo.Query.Fields.AddRange(RemoveDuplicateViewers(additionalSchemaInfo.Query.Fields));
                    schemaInfo.Query.Fields = schemaInfo.Query.Fields.OrderBy(f => f.Name).ToList();
                }
                else
                {
                    schemaInfo.Query = additionalSchemaInfo.Query;
                }

                if (schemaInfo.Mutation != null)
                {
                    schemaInfo.Mutation.Fields.AddRange(RemoveDuplicateViewers(additionalSchemaInfo.Mutation.Fields));
                    schemaInfo.Mutation.Fields = schemaInfo.Mutation.Fields.OrderBy(f => f.Name).ToList();
                }
                else
                {
                    schemaInfo.Mutation = additionalSchemaInfo.Mutation;
                }

                if (schemaInfo.Subscription != null)
                {
                    schemaInfo.Subscription.Fields.AddRange(RemoveDuplicateViewers(additionalSchemaInfo.Subscription.Fields));
                    schemaInfo.Subscription.Fields = schemaInfo.Subscription.Fields.OrderBy(f => f.Name).ToList();
                }
                else
                {
                    schemaInfo.Subscription = additionalSchemaInfo.Subscription;
                }

                schemaInfo.Types.AddRange(additionalSchemaInfo.Types.Where(t => !schemaInfo.Types.Contains(t)));
            }

            if (schemaInfos.Count > 1)
            {
                if (schemaInfo.Query != null)
                {
                    schemaInfo.Query.Name = "Query";
                }
                if (schemaInfo.Mutation != null)
                {
                    schemaInfo.Mutation.Name = "Mutation";
                }
                if (schemaInfo.Subscription != null)
                {
                    schemaInfo.Subscription.Name = "Subscription";
                }
            }

            return schemaInfo != null
                ? _graphTypeAdapter.DeriveSchema(schemaInfo)
                : null;
        }

        private IEnumerable<GraphFieldInfo> RemoveDuplicateViewers(IEnumerable<GraphFieldInfo> fields)
        {
            var registeredViewerClasses = new[]
            {
                typeof(ImplementViewerAttribute.QueryViewerReferrer),
                typeof(ImplementViewerAttribute.MutationViewerReferrer),
                typeof(ImplementViewerAttribute.SubscriptionViewerReferrer),
            };

            foreach (var field in fields)
            {
                if (field.Name == "viewer" &&
                    registeredViewerClasses.Contains(field.DeclaringType.TypeRepresentation.AsType()))
                {
                    continue;
                }
                yield return field;
            }
        }
    }
}
