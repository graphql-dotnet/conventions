using System.Linq;
using GraphQL.Conventions;
using Tests.Templates;
using Tests.Templates.Extensions;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace Tests.Attributes.MetaData
{
    public class InjectAttributeTests : TestBase
    {
        [Test]
        public void Arguments_Can_Be_Injected()
        {
            var field = TypeInfo<ArgumentData>().Fields.First();
            field.Arguments.Count.ShouldEqual(3);
            field.ShouldHaveArgumentWithName("arg1").AndNotFlaggedAsInjected();
            field.ShouldHaveArgumentWithName("arg2").AndFlaggedAsInjected();
            field.ShouldHaveArgumentWithName("arg3").AndNotFlaggedAsInjected();
        }

        private class ArgumentData
        {
            public int Field(string arg1, [Inject] bool arg2, int arg3) { return 0; }
        }
    }
}
