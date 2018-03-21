using GraphQL.Conventions;
using GraphQL.Conventions.Tests;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Types;

namespace Tests.Attributes.MetaData
{
    public class NonNullAttributeTests : ConstructionTestBase
    {
        private readonly TypeResolver _typeResolver = new TypeResolver();

        private readonly IObjectGraphType _type;

        public NonNullAttributeTests()
        {
            _type = Type(_typeResolver.DeriveType<Arguments>()) as IObjectGraphType;
        }

        [Test]
        public void Fields_Can_Be_Attributed_NonNull()
        {
            _type.ShouldHaveFieldWithName("normalField").Type.ShouldEqual(typeof(StringGraphType));
            _type.ShouldHaveFieldWithName("unNullableField").Type.ShouldEqual(typeof(NonNullGraphType<StringGraphType>));
        }

        class Arguments
        {
            public string NormalField { get; set; }

            [NonNull]
            public string UnNullableField { get; set; }
        }
    }
}
