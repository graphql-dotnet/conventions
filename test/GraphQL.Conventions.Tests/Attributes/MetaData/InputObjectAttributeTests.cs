using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using Xunit;

namespace GraphQL.Conventions.Tests.Attributes.MetaData
{
    public class InputObjectAttributeTests : TestBase
    {
        [Fact]
        public void Types_Are_Correctly_Marked_As_Output_Types()
        {
            var type = TypeInfo<TestOutputObject>();
            type.IsOutputType.ShouldEqual(true);
            type.IsInputType.ShouldEqual(false);
        }

        [Fact]
        public void Types_Are_Correctly_Marked_As_Input_Types()
        {
            var type = TypeInfo<TestInputObject>();
            type.IsOutputType.ShouldEqual(false);
            type.IsInputType.ShouldEqual(true);
        }

        [Fact]
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
