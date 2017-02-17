using System.Linq;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Attributes.MetaData
{
    public class DeprecatedAttributeTests : TestBase
    {
        [Test]
        public void Types_Have_Correct_DeprecationReasons()
        {
            TypeInfo<NormalType>().ShouldNotBeDeprecated();
            TypeInfo<DeprecatedType>().ShouldBeDeprecatedWithReason("Some type reason");
        }

        [Test]
        public void Interfaces_Have_Correct_DeprecationReasons()
        {
            TypeInfo<NormalInterface>().ShouldNotBeDeprecated();
            TypeInfo<DeprecatedInterface>().ShouldBeDeprecatedWithReason("Some interface reason");
        }

        [Test]
        public void GenericTypes_Have_Correct_DeprecationReasons()
        {
            TypeInfo<NormalGenericType<string>>().ShouldNotBeDeprecated();
            TypeInfo<DeprecatedGenericType<string>>().ShouldBeDeprecatedWithReason("Some generic type reason");
        }

        [Test]
        public void Fields_Have_Correct_DeprecationReasons()
        {
            var type = TypeInfo<FieldData>();
            type.ShouldHaveFieldWithName("normalField").AndWithoutDeprecationReason();
            type.ShouldHaveFieldWithName("deprecatedField").AndWithDeprecationReason("Some field reason");
        }

        [Test]
        public void Arguments_Have_Correct_DeprecationReasons()
        {
            var field = TypeInfo<ArgumentData>().Fields.First();
            field.ShouldHaveArgumentWithName("arg1").AndWithoutDeprecationReason();
            field.ShouldHaveArgumentWithName("arg2").AndWithDeprecationReason("Some argument reason");
        }

        [Test]
        public void Enum_Members_Have_Correct_DeprecationReasons()
        {
            var type = TypeInfo<EnumData.Enum>();
            type.ShouldHaveFieldWithName("NORMAL_MEMBER").AndWithoutDeprecationReason();
            type.ShouldHaveFieldWithName("DEPRECATED_MEMBER").AndWithDeprecationReason("Some enum member reason");
        }

        class NormalType { }

        [Deprecated("Some type reason")]
        class DeprecatedType { }

        interface NormalInterface { }

        [Deprecated("Some interface reason")]
        interface DeprecatedInterface { }

        class NormalGenericType<T> { }

        [Deprecated("Some generic type reason")]
        class DeprecatedGenericType<T> { }

        class FieldData
        {
            public int NormalField { get; set; }

            [Deprecated("Some field reason")]
            public bool DeprecatedField { get; set; }
        }

        class ArgumentData
        {
            public void Field(bool arg1, [Deprecated("Some argument reason")] bool arg2) { }
        }

        class EnumData
        {
            public enum Enum
            {
                NormalMember,

                [Deprecated("Some enum member reason")]
                DeprecatedMember,
            }
        }
    }
}
