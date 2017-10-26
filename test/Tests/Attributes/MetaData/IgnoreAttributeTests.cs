using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types.Resolution;
using System;

namespace GraphQL.Conventions.Tests.Attributes.MetaData
{
    public class IgnoreAttributeTests : TestBase, IDisposable
    {
        [Test]
        public void Fields_Can_Be_Ignored()
        {
            var type = TypeInfo<FieldData>();
            type.Fields.Count.ShouldEqual(2);
            type.ShouldHaveFieldWithName("normalField");
            type.ShouldHaveFieldWithName("fieldWithVoidReturnType");
            type.ShouldNotHaveFieldWithName("ignoredField");
        }

        [Test]
        public void Fields_With_Void_Return_Type_Can_Be_Ignored()
        {
            var resolver = new TypeResolver();
            var type = resolver.IgnoreFieldsWithVoidReturnType().DeriveType<FieldData>();

            type.Fields.Count.ShouldEqual(1);
            type.ShouldHaveFieldWithName("normalField");
            type.ShouldNotHaveFieldWithName("fieldWithVoidReturnType");
            type.ShouldNotHaveFieldWithName("ignoredField");
        }

        [Test]
        public void Enum_Members_Can_Be_Ignored()
        {
            var type = TypeInfo<EnumData.Enum>();
            type.ShouldHaveFieldWithName("NORMAL_MEMBER").AndWithoutDeprecationReason();
            type.ShouldHaveFieldWithName("DEPRECATED_MEMBER").AndWithDeprecationReason("Some enum member reason");
        }

        public void Dispose()
        {
            ReflectorSettingsExtensions.ResetIgnoreFieldWithVoidReturnType();
        }

        class FieldData
        {
            public int NormalField { get; set; }

            [Ignore]
            public bool IgnoredField { get; set; }

            public void FieldWithVoidReturnType (int input) { }
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
