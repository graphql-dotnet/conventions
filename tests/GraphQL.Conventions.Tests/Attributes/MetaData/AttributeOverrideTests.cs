using GraphQL.Conventions;
using Tests.Templates;
using Tests.Templates.Extensions;

namespace Tests.Attributes.MetaData
{
    public class AttributeOverrideTests : TestBase
    {
        [Test]
        public void Attributes_Can_Be_Set()
        {
            TypeInfo<FirstClass>().Name.ShouldEqual("First");
        }

        [Test]
        public void Attributes_Can_Be_Overridden()
        {
            TypeInfo<SecondClass>().Name.ShouldEqual("Second");
        }

        [Test]
        public void Attributes_Can_Be_Double_Overridden()
        {
            TypeInfo<ThirdClass>().Name.ShouldEqual("Third");
        }

        [Test]
        public void Default_Attributes_Cannot_Override_Inherited_Attributes()
        {
            TypeInfo<ForthClass>().Name.ShouldEqual("Third");
        }

        [Name("First")]
        private class FirstClass
        {
        }

        [Name("Second")]
        private class SecondClass : FirstClass
        {
        }

        [Name("Third")]
        private class ThirdClass : SecondClass
        {
        }

        private class ForthClass : ThirdClass
        {
        }
    }
}
