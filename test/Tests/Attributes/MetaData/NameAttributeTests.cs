using System;
using System.Linq;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Attributes.MetaData
{
    public class NameAttributeTests : TestBase
    {
        [Test]
        public void Types_Have_Correct_Names()
        {
            TypeInfo<NormalType>().ShouldBeNamed("NormalType");
            TypeInfo<OverriddenType>().ShouldBeNamed("SomeType");
        }

        [Test]
        public void Interfaces_Have_Correct_Names()
        {
            TypeInfo<NormalInterface>().ShouldBeNamed("NormalInterface");
            TypeInfo<OverriddenInterface>().ShouldBeNamed("SomeInterface");
        }

        [Test]
        public void GenericTypes_Have_Correct_Names()
        {
            TypeInfo<GenericType<string>>().ShouldBeNamed("StringGenericType");
            TypeInfo<OverriddenGenericType<string>>().ShouldBeNamed("SomeGenericType");
        }

        [Test]
        public void Fields_Have_Correct_Names()
        {
            var type = TypeInfo<FieldData>();
            type.ShouldHaveFieldWithName("field1");
            type.ShouldHaveFieldWithName("field2");
            type.ShouldHaveFieldWithName("overridden1");
            type.ShouldHaveFieldWithName("overridden2");
            type.ShouldNotHaveFieldWithName("overriddenField1");
            type.ShouldNotHaveFieldWithName("overriddenField2");
        }

        [Test]
        public void Arguments_Have_Correct_Names()
        {
            var field = TypeInfo<ArgumentData>().Fields.First();
            field.ShouldHaveArgumentWithName("normalArg");
            field.ShouldHaveArgumentWithName("someArg");
            field.ShouldNotHaveArgumentWithName("overriddenArg");
        }

        [Test]
        public void Enum_Members_Have_Correct_Names()
        {
            var type = TypeInfo<EnumData.Enum>();
            type.ShouldHaveFieldWithName("NORMAL_MEMBER");
            type.ShouldHaveFieldWithName("SOME_MEMBER");
            type.ShouldNotHaveFieldWithName("OVERRIDDEN_MEMBER");
        }

        class NormalType { }

        [Name("SomeType")]
        class OverriddenType { }

        interface NormalInterface { }

        [Name("SomeInterface")]
        interface OverriddenInterface { }

        class GenericType<T> { }

        [Name("SomeGenericType")]
        class OverriddenGenericType<T> { }

        class FieldData
        {
            public int Field1 { get; set; }

            public string Field2() { return string.Empty; }

            [Name("overridden1")]
            public bool OverriddenField1 { get; set; }

            [Name("overridden2")]
            public DateTime OverriddenField2() { return DateTime.UtcNow; }
        }

        class ArgumentData
        {
            public void Field(bool normalArg, [Name("someArg")] bool overriddenArg) { }
        }

        class EnumData
        {
            public enum Enum
            {
                NormalMember,

                [Name("SOME_MEMBER")]
                OverriddenMember,
            }
        }
    }
}
