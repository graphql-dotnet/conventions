using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using Xunit;

namespace GraphQL.Conventions.Tests.Attributes.MetaData
{
    public class AttributeOverrideTests : TestBase
    {
        [Fact]
        public void Attributes_Can_Be_Set()
        {
            TypeInfo<FirstClass>().Name.ShouldEqual("First");
        }

        [Fact]
        public void Attributes_Can_Be_Overridden()
        {
            TypeInfo<SecondClass>().Name.ShouldEqual("Second");
        }

        [Fact]
        public void Attributes_Can_Be_Double_Overridden()
        {
            TypeInfo<ThirdClass>().Name.ShouldEqual("Third");
        }

        [Fact]
        public void Default_Attributes_Cannot_Override_Inherited_Attributes()
        {
            TypeInfo<ForthClass>().Name.ShouldEqual("Third");
        }

        [Name("First")]
        class FirstClass
        {
        }

        [Name("Second")]
        class SecondClass : FirstClass
        {
        }

        [Name("Third")]
        class ThirdClass : SecondClass
        {
        }

        class ForthClass : ThirdClass
        {
        }
    }
}
