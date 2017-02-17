using System.Reflection;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.MetaData.Utilities
{
    public interface IMappableTarget
    {
        void MapArgument(GraphArgumentInfo entity, ParameterInfo parameterInfo);

        void MapField(GraphFieldInfo entity, MemberInfo memberInfo);

        void MapEnumMember(GraphFieldInfo entity, FieldInfo fieldInfo);

        void MapType(GraphTypeInfo entity, TypeInfo typeInfo);
    }
}
