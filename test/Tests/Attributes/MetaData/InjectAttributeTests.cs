using System.Linq;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Attributes.MetaData
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

        class ArgumentData
        {
            public void Field(string arg1, [Inject] bool arg2, int arg3) { }
        }
    }
}
