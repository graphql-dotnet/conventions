using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Attributes.MetaData
{
    public class IgnoreAttributeTests : TestBase
    {
        [Test]
        public void Fields_Can_Be_Ignored()
        {
            var type = TypeInfo<FieldData>();
            type.Fields.Count.ShouldEqual(1);
            type.ShouldHaveFieldWithName("normalField");
            type.ShouldNotHaveFieldWithName("ignoredField");
        }

        [Test]
        public void Enum_Members_Can_Be_Ignored()
        {
            var type = TypeInfo<EnumData.Enum>();
            type.ShouldHaveFieldWithName("NORMAL_MEMBER").AndWithoutDeprecationReason();
            type.ShouldHaveFieldWithName("DEPRECATED_MEMBER").AndWithDeprecationReason("Some enum member reason");
        }

        class FieldData
        {
            public int NormalField { get; set; }

            [Ignore]
            public bool IgnoredField { get; set; }
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
