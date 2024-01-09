using System;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Descriptors;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions.Relay
{
    [AttributeUsage(Types, AllowMultiple = true)]
    public class ImplementViewerAttribute : MetaDataAttributeBase
    {
        private readonly OperationType _operationType;

        public ImplementViewerAttribute(OperationType operationType)
            : base(AttributeApplicationPhase.Override)
        {
            _operationType = operationType;
        }

        public override void MapType(GraphTypeInfo type, TypeInfo typeInfo)
        {
            TypeInfo viewer;
            TypeInfo viewerReferrer;

            switch (_operationType)
            {
                case OperationType.Query:
                    viewer = typeof(QueryViewer).GetTypeInfo();
                    viewerReferrer = typeof(QueryViewerReferrer).GetTypeInfo();
                    break;
                case OperationType.Mutation:
                    viewer = typeof(MutationViewer).GetTypeInfo();
                    viewerReferrer = typeof(MutationViewerReferrer).GetTypeInfo();
                    break;
                case OperationType.Subscription:
                    viewer = typeof(SubscriptionViewer).GetTypeInfo();
                    viewerReferrer = typeof(SubscriptionViewerReferrer).GetTypeInfo();
                    break;
                default:
                    return;
            }

            var viewerType = type.TypeResolver.DeriveType(viewer);
            viewerType.Fields.AddRange(type.Fields);
            viewerType.Fields = viewerType.Fields.OrderBy(f => f.Name).ToList();

            var viewerReferrerType = type.TypeResolver.DeriveType(viewerReferrer);
            var viewerField = viewerReferrerType.Fields.First(field => field.Name == "viewer");
            type.Fields.Add(viewerField);
            type.Fields = type.Fields.OrderBy(f => f.Name).ToList();
        }

        public class QueryViewer
        {
        }

        public class QueryViewerReferrer
        {
            public QueryViewer Viewer => new QueryViewer();
        }

        public class MutationViewer
        {
        }

        public class MutationViewerReferrer
        {
            public MutationViewer Viewer => new MutationViewer();
        }

        public class SubscriptionViewer
        {
        }

        public class SubscriptionViewerReferrer
        {
            public SubscriptionViewer Viewer => new SubscriptionViewer();
        }
    }
}
