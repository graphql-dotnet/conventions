using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Attributes.MetaData
{
    public class InputObjectAttributeTests : TestBase
    {
        [Test]
        public void Types_Are_Correctly_Marked_As_Output_Types()
        {
            var type = TypeInfo<TestOutputObject>();
            type.IsOutputType.ShouldEqual(true);
            type.IsInputType.ShouldEqual(false);
        }

        [Test]
        public void Types_Are_Correctly_Marked_As_Input_Types()
        {
            var type = TypeInfo<TestInputObject>();
            type.IsOutputType.ShouldEqual(false);
            type.IsInputType.ShouldEqual(true);
        }

        [Test]
        public void Structs_Are_Correctly_Marked_As_Value_Types()
        {
            var type = TypeInfo<TestStruct>();
            type.IsOutputType.ShouldEqual(true);
            type.IsInputType.ShouldEqual(true);
        }

        class TestOutputObject
        {
        }

        [InputType]
        class TestInputObject
        {
        }

        struct TestStruct
        {
        }
    }
}
