using System;
using System.Reflection;
using GraphQL.Conventions.Attributes.Collectors;
using GraphQL.Conventions.Handlers;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(Everywhere, AllowMultiple = true, Inherited = true)]
    public class CoreAttribute : MetaDataAttributeBase, IDefaultAttribute
    {
        private readonly ExecutionFilterAttributeHandler _executionFilterHandler =
            new ExecutionFilterAttributeHandler();

        public CoreAttribute()
            : base(AttributeApplicationPhase.Initialization)
        {
        }

        public override void MapType(GraphTypeInfo entity, TypeInfo typeInfo)
        {
            var typeRepresentation = typeInfo.GetTypeRepresentation();
            if (typeRepresentation.IsSubclassOf(typeof(Union)))
            {
                DeclareUnionType(entity, typeRepresentation);
            }
        }

        public override void MapField(GraphFieldInfo entity, MemberInfo memberInfo)
        {
            if (memberInfo is MethodInfo)
            {
                entity.ResolveDelegate =
                    context => entity.TypeResolver.FieldResolver.CallMethod(entity, context);
            }
            else if (memberInfo is PropertyInfo)
            {
                entity.ResolveDelegate =
                    context => entity.TypeResolver.FieldResolver.GetValue(entity, context);
            }
        }

        private void DeclareUnionType(GraphTypeInfo entity, TypeInfo typeInfo)
        {
            var unionType = typeInfo.BaseType.GetTypeInfo();
            if (unionType != null &&
                unionType.Name.StartsWith(nameof(Union)) &&
                unionType.IsSubclassOf(typeof(Union)) &&
                unionType.IsGenericType)
            {
                foreach (var type in unionType.GenericTypeArguments)
                {
                    entity.AddUnionType(entity.TypeResolver.DeriveType(type.GetTypeInfo()));
                }
            }
        }
    }
}
