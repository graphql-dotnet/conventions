using System.Linq;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Types;

namespace GraphQL.Conventions.Tests.Adapters
{
    public class ArgumentDerivationTests : ConstructionTestBase
    {
        private readonly TypeResolver _typeResolver = new TypeResolver();

        private readonly IObjectGraphType _type;

        public ArgumentDerivationTests()
        {
            _type = Type(_typeResolver.DeriveType<Arguments>()) as IObjectGraphType;
        }

        [Test]
        public void Can_Derive_Field_Without_Arguments()
        {
            var field = _type.ShouldHaveFieldWithName("fieldWithNoArguments");
            field.Arguments.Count.ShouldEqual(0);
        }

        [Test]
        public void Can_Derive_Field_With_Arguments()
        {
            var field = _type.ShouldHaveFieldWithName("fieldWithArguments");
            field.Arguments.Count.ShouldEqual(1);
            field.Arguments.First().Name.ShouldEqual("a");
            field.Arguments.First().DefaultValue.ShouldEqual(null);
        }


        [Test]
        public void Can_Derive_Field_With_Arguments_With_Default_Value()
        {
            var field = _type.ShouldHaveFieldWithName("fieldWithArgumentsWithDefaultValue");
            field.Arguments.Count.ShouldEqual(1);
            field.Arguments.First().Name.ShouldEqual("a");
            field.Arguments.First().DefaultValue.ShouldEqual(99);
        }

        [Test]
        public void Can_Derive_Field_With_Named_Arguments()
        {
            var field = _type.ShouldHaveFieldWithName("fieldWithNamedArgument");
            field.Arguments.Count.ShouldEqual(1);
            field.Arguments.First().Name.ShouldEqual("overriddenName");
        }

        [Test]
        public void Can_Derive_Field_With_Described_Arguments()
        {
            var field = _type.ShouldHaveFieldWithName("fieldWithDescribedArgument");
            field.Arguments.Count.ShouldEqual(2);
            field.Arguments[0].Description.ShouldBeEmpty();
            field.Arguments[1].Description.ShouldEqual("Some Description");
        }

        [Test]
        public void Can_Derive_Field_With_Injected_Arguments()
        {
            var field = _type.ShouldHaveFieldWithName("fieldWithInjectedArgument");
            field.Arguments.Count.ShouldEqual(1);
        }

        [Test]
        public void Can_Derive_Unwrapped_Type_Of_Field_Argument()
        {
            var field = _type.ShouldHaveFieldWithName("fieldWithUnwrappedType");
            field.Arguments.First().Type.ShouldEqual(typeof(StringGraphType));
        }

        [Test]
        public void Can_Derive_Wrapped_Type_Of_Field_Argument()
        {
            var field = _type.ShouldHaveFieldWithName("fieldWithWrappedType");
            field.Arguments.First().Type.ShouldEqual(typeof(NonNullGraphType<StringGraphType>));
        }

        class Arguments
        {
            public int FieldWithNoArguments() => 0;

            public int FieldWithArguments(int a) => 0;

            public int FieldWithArgumentsWithDefaultValue(int a = 99) => 0;

            public int FieldWithNamedArgument([Name("overriddenName")] int a) => 0;

            public int FieldWithDescribedArgument(int a, [Description("Some Description")] int b) => 0;

            public int FieldWithInjectedArgument(int a, [Inject] int injected) => 0;

            public int FieldWithUnwrappedType(string a) => 0;

            public int FieldWithWrappedType(NonNull<string> a) => 0;
        }
    }
}