using System.Reflection;
using GraphQL.Conventions.Attributes.MetaData.Utilities;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Tests.Attributes.MetaData.Utilities
{
    public class EntityMapperTests : TestBase
    {
        [Test]
        public void Can_Map_Arguments()
        {
            var target = new MappableTarget();
            var entityMapper = new EntityMapper(target);
            var typeResolver = new TypeResolver();
            var argumentInfo = typeof(TestObject).GetTypeInfo().GetMethod(nameof(TestObject.Field)).GetParameters()[0];
            entityMapper.MapEntity(new GraphArgumentInfo(typeResolver, argumentInfo));
            target.HasMappedArgument.ShouldEqual(true);
            target.HasMappedEnumMember.ShouldEqual(false);
            target.HasMappedField.ShouldEqual(false);
            target.HasMappedType.ShouldEqual(false);
        }

        [Test]
        public void Can_Map_Enum_Members()
        {
            var target = new MappableTarget();
            var entityMapper = new EntityMapper(target);
            var typeResolver = new TypeResolver();
            var memberInfo = typeof(TestEnum).GetTypeInfo().GetField(nameof(TestEnum.Member));
            entityMapper.MapEntity(new GraphEnumMemberInfo(typeResolver, memberInfo));
            target.HasMappedArgument.ShouldEqual(false);
            target.HasMappedEnumMember.ShouldEqual(true);
            target.HasMappedField.ShouldEqual(false);
            target.HasMappedType.ShouldEqual(false);
        }

        [Test]
        public void Can_Map_Fields()
        {
            var target = new MappableTarget();
            var entityMapper = new EntityMapper(target);
            var typeResolver = new TypeResolver();
            var memberInfo = typeof(TestObject).GetTypeInfo().GetMethod(nameof(TestObject.Field));
            entityMapper.MapEntity(new GraphFieldInfo(typeResolver, memberInfo));
            target.HasMappedArgument.ShouldEqual(false);
            target.HasMappedEnumMember.ShouldEqual(false);
            target.HasMappedField.ShouldEqual(true);
            target.HasMappedType.ShouldEqual(false);
        }

        [Test]
        public void Can_Map_Types()
        {
            var target = new MappableTarget();
            var entityMapper = new EntityMapper(target);
            var typeResolver = new TypeResolver();
            var typeInfo = typeof(TestObject).GetTypeInfo();
            entityMapper.MapEntity(new GraphTypeInfo(typeResolver, typeInfo));
            target.HasMappedArgument.ShouldEqual(false);
            target.HasMappedEnumMember.ShouldEqual(false);
            target.HasMappedField.ShouldEqual(false);
            target.HasMappedType.ShouldEqual(true);
        }

        class TestObject
        {
            public int Field(int argument) => argument;
        }

        enum TestEnum
        {
            Member,
        }

        class MappableTarget : IMappableTarget
        {
            public bool HasMappedArgument { get; private set; }

            public bool HasMappedEnumMember { get; private set; }

            public bool HasMappedField { get; private set; }

            public bool HasMappedType { get; private set; }

            public void MapArgument(GraphArgumentInfo entity, ParameterInfo parameterInfo)
            {
                HasMappedArgument = true;
            }

            public void MapEnumMember(GraphFieldInfo entity, FieldInfo fieldInfo)
            {
                HasMappedEnumMember = true;
            }

            public void MapField(GraphFieldInfo entity, MemberInfo memberInfo)
            {
                HasMappedField = true;
            }

            public void MapType(GraphTypeInfo entity, TypeInfo typeInfo)
            {
                HasMappedType = true;
            }
        }
    }
}
