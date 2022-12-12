using System.Linq;
using GraphQL.Conventions;
using Tests.Templates;
using Tests.Templates.Extensions;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedTypeParameter
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace Tests.Attributes.MetaData
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

        private class NormalType { }

        [Deprecated("Some type reason")]
        private class DeprecatedType { }

        private interface NormalInterface { }

        [Deprecated("Some interface reason")]
        private interface DeprecatedInterface { }

        private class NormalGenericType<T> { }

        [Deprecated("Some generic type reason")]
        private class DeprecatedGenericType<T> { }

        private class FieldData
        {
            public int NormalField { get; set; }

            [Deprecated("Some field reason")]
            public bool DeprecatedField { get; set; }
        }

        private class ArgumentData
        {
            public int Field(bool arg1, [Deprecated("Some argument reason")] bool arg2) { return 0; }
        }

        private class EnumData
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
