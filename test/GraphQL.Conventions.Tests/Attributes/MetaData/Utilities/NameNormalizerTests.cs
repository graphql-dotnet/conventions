using GraphQL.Conventions.Attributes.MetaData.Utilities;
using Tests.Templates.Extensions;

namespace Tests.Attributes.MetaData.Utilities
{
    public class NameNormalizerTests
    {
        [Test]
        public void Can_Derive_Correct_Type_Names()
        {
            AsTypeName(null).ShouldEqual("");
            AsTypeName("").ShouldEqual("");
            AsTypeName("Type").ShouldEqual("Type");
            AsTypeName("type").ShouldEqual("Type");
            AsTypeName("TYPE").ShouldEqual("TYPE");
            AsTypeName("SomeType").ShouldEqual("SomeType");
            AsTypeName("someType").ShouldEqual("SomeType");
            AsTypeName("sometype").ShouldEqual("Sometype");
            AsTypeName("SOMETYPE").ShouldEqual("SOMETYPE");
            AsTypeName("Type`1").ShouldEqual("Type");
            AsTypeName("SomeType`1").ShouldEqual("SomeType");
            AsTypeName("someType`1").ShouldEqual("SomeType");
        }

        [Test]
        public void Can_Derive_Correct_Field_Names()
        {
            AsFieldName(null).ShouldEqual("");
            AsFieldName("").ShouldEqual("");
            AsFieldName("Field").ShouldEqual("field");
            AsFieldName("field").ShouldEqual("field");
            AsFieldName("FIELD").ShouldEqual("fIELD");
            AsFieldName("SomeField").ShouldEqual("someField");
            AsFieldName("someField").ShouldEqual("someField");
            AsFieldName("somefield").ShouldEqual("somefield");
            AsFieldName("SOMEFIELD").ShouldEqual("sOMEFIELD");
            AsFieldName("Field`1").ShouldEqual("field");
            AsFieldName("SomeField`1").ShouldEqual("someField");
            AsFieldName("someField`1").ShouldEqual("someField");
        }

        [Test]
        public void Can_Derive_Correct_Argument_Names()
        {
            AsArgumentName(null).ShouldEqual("");
            AsArgumentName("").ShouldEqual("");
            AsArgumentName("Argument").ShouldEqual("argument");
            AsArgumentName("argument").ShouldEqual("argument");
            AsArgumentName("ARGUMENT").ShouldEqual("aRGUMENT");
            AsArgumentName("SomeArgument").ShouldEqual("someArgument");
            AsArgumentName("someArgument").ShouldEqual("someArgument");
            AsArgumentName("someargument").ShouldEqual("someargument");
            AsArgumentName("SOMEARGUMENT").ShouldEqual("sOMEARGUMENT");
            AsArgumentName("Argument`1").ShouldEqual("argument");
            AsArgumentName("SomeArgument`1").ShouldEqual("someArgument");
            AsArgumentName("someArgument`1").ShouldEqual("someArgument");
        }

        [Test]
        public void Can_Derive_Correct_Enum_Member_Names()
        {
            AsEnumMemberName(null).ShouldEqual("");
            AsEnumMemberName("").ShouldEqual("");
            AsEnumMemberName("Enum").ShouldEqual("ENUM");
            AsEnumMemberName("enum").ShouldEqual("ENUM");
            AsEnumMemberName("ENUM").ShouldEqual("ENUM");
            AsEnumMemberName("SomeEnum").ShouldEqual("SOME_ENUM");
            AsEnumMemberName("someEnum").ShouldEqual("SOME_ENUM");
            AsEnumMemberName("someenum").ShouldEqual("SOMEENUM");
            AsEnumMemberName("SOMEENUM").ShouldEqual("SOMEENUM");
            AsEnumMemberName("Enum`1").ShouldEqual("ENUM");
            AsEnumMemberName("SomeEnum`1").ShouldEqual("SOME_ENUM");
            AsEnumMemberName("someEnum`1").ShouldEqual("SOME_ENUM");
        }

        private readonly INameNormalizer _normalizer = new NameNormalizer();

        private string AsTypeName(string name) => _normalizer.AsTypeName(name);

        private string AsFieldName(string name) =>  _normalizer.AsFieldName(name);

        private string AsArgumentName(string name) =>  _normalizer.AsArgumentName(name);

        private string AsEnumMemberName(string name) =>  _normalizer.AsEnumMemberName(name);
    }
}
