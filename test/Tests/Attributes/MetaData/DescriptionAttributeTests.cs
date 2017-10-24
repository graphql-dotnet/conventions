using System.Linq;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Attributes.MetaData
{
    public class DescriptionAttributeTests : TestBase
    {
        [Test]
        public void Types_Have_Correct_Descriptions()
        {
            TypeInfo<UndescribedType>().ShouldNotBeDescribed();
            TypeInfo<DescribedType>().ShouldHaveDescription("Some type description");
        }

        [Test]
        public void Interfaces_Have_Correct_Descriptions()
        {
            TypeInfo<UndescribedInterface>().ShouldNotBeDescribed();
            TypeInfo<DescribedInterface>().ShouldHaveDescription("Some interface description");
        }

        [Test]
        public void GenericTypes_Have_Correct_Descriptions()
        {
            TypeInfo<UndescribedGenericType<string>>().ShouldNotBeDescribed();
            TypeInfo<DescribedGenericType<string>>().ShouldHaveDescription("Some generic type description");
        }

        [Test]
        public void Fields_Have_Correct_Descriptions()
        {
            var type = TypeInfo<FieldData>();
            type.ShouldHaveFieldWithName("undescribedField").AndWithoutDescription();
            type.ShouldHaveFieldWithName("describedField").AndWithDescription("Some field description");
        }

        [Test]
        public void Arguments_Have_Correct_Descriptions()
        {
            var field = TypeInfo<ArgumentData>().Fields.First();
            field.ShouldHaveArgumentWithName("arg1").AndWithoutDescription();
            field.ShouldHaveArgumentWithName("arg2").AndWithDescription("Some argument description");
        }

        [Test]
        public void Enum_Members_Have_Correct_Descriptions()
        {
            var type = TypeInfo<EnumData.Enum>();
            type.ShouldHaveFieldWithName("UNDESCRIBED_MEMBER").AndWithoutDescription();
            type.ShouldHaveFieldWithName("DESCRIBED_MEMBER").AndWithDescription("Some enum member description");
        }

        class UndescribedType { }

        [Description("Some type description")]
        class DescribedType { }

        interface UndescribedInterface { }

        [Description("Some interface description")]
        interface DescribedInterface { }

        class UndescribedGenericType<T> { }

        [Description("Some generic type description")]
        class DescribedGenericType<T> { }

        class FieldData
        {
            public int UndescribedField { get; set; }

            [Description("Some field description")]
            public bool DescribedField { get; set; }
        }

        class ArgumentData
        {
            public int Field(bool arg1, [Description("Some argument description")] bool arg2) { return 0; }
        }

        class EnumData
        {
            public enum Enum
            {
                UndescribedMember,

                [Description("Some enum member description")]
                DescribedMember,
            }
        }
    }
}
