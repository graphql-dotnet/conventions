using System.Linq;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Types;
using Xunit;
using Extended = GraphQL.Conventions.Adapters.Types;

namespace GraphQL.Conventions.Tests.Adapters
{
    public class FieldDerivationTests : ConstructionTestBase
    {
        [Fact]
        public void Can_Derive_Output_Type_With_No_Fields()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<OutputTypeWithNoFields>()) as IObjectGraphType;
            type.ShouldHaveFields(0);
        }

        [Fact]
        public void Can_Derive_Input_Type_With_No_Fields()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<InputTypeWithNoFields>()) as InputObjectGraphType;
            type.ShouldHaveFields(0);
        }

        [Fact]
        public void Can_Derive_Output_Type_With_Fields()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<OutputTypeWithFields>()) as IObjectGraphType;
            type.ShouldHaveFields(2);
        }

        [Fact]
        public void Can_Derive_Input_Type_With_Fields()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<InputTypeWithFields>()) as InputObjectGraphType;
            type.ShouldHaveFields(2);
        }

        [Fact]
        public void Can_Hide_Ignored_Field()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<Fields>()) as IObjectGraphType;
            type.ShouldNotHaveFieldWithName("ignoredField");
        }

        [Fact]
        public void Can_Derive_Normal_Field()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<Fields>()) as IObjectGraphType;
            var field = type.ShouldHaveFieldWithName("normalField");
            field.Arguments.Count.ShouldEqual(0);
        }

        [Fact]
        public void Can_Derive_Field_With_Overridden_Name()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<Fields>()) as IObjectGraphType;
            type.ShouldHaveFieldWithName("overriddenField");
        }

        [Fact]
        public void Can_Derive_Field_With_Description()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<Fields>()) as IObjectGraphType;
            var field = type.ShouldHaveFieldWithName("fieldWithDescription");
            field.Description.ShouldEqual("Some Description");
        }

        [Fact]
        public void Can_Derive_Field_With_Deprecation_Reason()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<Fields>()) as IObjectGraphType;
            var field = type.ShouldHaveFieldWithName("fieldWithDeprecationReason");
            field.DeprecationReason.ShouldEqual("Some Deprecation Reason");
        }

        [Fact]
        public void Can_Derive_Field_Type_When_Unwrapped()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<Fields>()) as IObjectGraphType;
            var field = type.ShouldHaveFieldWithName("stringField");
            field.Type.ShouldEqual(typeof(StringGraphType));
        }

        [Fact]
        public void Can_Derive_Field_Type_When_Wrapped()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<Fields>()) as IObjectGraphType;
            var field = type.ShouldHaveFieldWithName("nonNullStringField");
            field.Type.ShouldEqual(typeof(NonNullGraphType<StringGraphType>));
        }

        [Fact]
        public void Can_Derive_Field_With_Arguments()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<Fields>()) as IObjectGraphType;
            var field = type.ShouldHaveFieldWithName("fieldWithArguments");
            field.Type.ShouldEqual(typeof(NonNullGraphType<IntGraphType>));
            field.Arguments.Count.ShouldEqual(2);
            field.ShouldHaveArgument("a");
            field.ShouldHaveArgument("b");
        }

        [Fact]
        public void Can_Override_Fields_In_Derived_Types()
        {
            var type = OutputType<Foo>();
            type.Name.ShouldEqual("Fooz");
            type.Description.ShouldEqual("Foo bar baz");
            type.ShouldHaveFields(3);
            type.ShouldHaveFieldWithName("id").OfType<NonNullGraphType<Extended.IdGraphType>>();
            type.ShouldHaveFieldWithName("a").OfType<IntGraphType>();
            type.ShouldHaveFieldWithName("b").OfType<NonNullGraphType<IntGraphType>>();
        }

        class OutputTypeWithNoFields
        {
        }

        [InputType]
        class InputTypeWithNoFields
        {
        }

        class OutputTypeWithFields
        {
            public int Field1 => 1;

            public int Field2 => 2;
        }

        [InputType]
        class InputTypeWithFields
        {
            public int Field1 { get; set; }

            public int Field2 { get; set; }
        }

        class Fields
        {
            public int NormalField => 0;

            [Name("overriddenField")]
            public int FieldWithOverriddenName => 0;

            [Description("Some Description")]
            public int FieldWithDescription => 0;

            [Deprecated("Some Deprecation Reason")]
            public int FieldWithDeprecationReason => 0;

            [Ignore]
            public int IgnoredField => 0;

            public string StringField => string.Empty;

            public NonNull<string> NonNullStringField => string.Empty;

            public int FieldWithArguments(int a, int b) => a + b;
        }

        [Name("Fooz")]
        class FooDto
        {
            public string Id => "A";

            public int? A => 0;

            public int B => 1;

            public string C => "Ignore";
        }

        [Description("Foo bar baz")]
        class Foo : FooDto
        {
            public new Id Id => base.Id;

            [Ignore]
            public new string C => string.Empty;
        }

    }
}