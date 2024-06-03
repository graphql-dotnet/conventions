using System.Linq;
using GraphQL.Conventions;
using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Builders;
using GraphQL.Types;
using Tests.Templates;
using Tests.Templates.Extensions;
// ReSharper disable UnusedMember.Local

namespace Tests.Attributes.MetaData
{
    public class IgnoreAttributeTests : TestBase
    {
        [Test]
        public void Fields_Can_Be_Ignored()
        {
            var type = TypeInfo<FieldData>();
            type.Fields.Count.ShouldEqual(1);
            type.ShouldHaveFieldWithName("normalField");
            type.ShouldNotHaveFieldWithName("ignoredField");
        }

        [Test]
        public void Enum_Members_Can_Be_Ignored()
        {
            var type = TypeInfo<EnumData.Enum>();
            type.ShouldHaveFieldWithName("NORMAL_MEMBER").AndWithoutDeprecationReason();
            type.ShouldHaveFieldWithName("DEPRECATED_MEMBER").AndWithDeprecationReason("Some enum member reason");
        }

        [Test]
        public void Derived_Fields_Can_Be_Ignored()
        {
            var type = TypeInfo<Implementation>();
            type.Fields.Count.ShouldEqual(2);
            type.ShouldHaveFieldWithName("firstProperty");
            type.ShouldHaveFieldWithName("someOtherProperty");
            type.ShouldNotHaveFieldWithName("someProperty");

            var iface = TypeInfo<ISomeInterface>();
            iface.Fields.Count.ShouldEqual(1);
            iface.ShouldHaveFieldWithName("firstProperty");
            iface.ShouldNotHaveFieldWithName("someProperty");
        }

        [Test]
        public void Can_Ignore_Unwanted_Fields_On_Derived_Types()
        {
            var schema = new SchemaConstructor<ISchema, IGraphType>(new GraphTypeAdapter())
                .IgnoreTypes((t, m) => m?.Name == nameof(ISomeInterfaceExternal.SomeProperty))
                .Build(typeof(SchemaType));

            schema.Initialize();
            var type = schema.AllTypes[nameof(ImplementationExternal)] as ObjectGraphType;
            Assert.AreEqual(1, type.Fields.Count(f => f.Name == "someOtherProperty"));
            Assert.AreEqual(0, type.Fields.Count(f => f.Name == "someProperty"));
        }

        private class SchemaType
        {
            public QueryType Query { get; }
        }

        private class QueryType
        {
            public ImplementationExternal Foo => new();
        }

        private class FieldData
        {
            public int NormalField { get; set; }

            [Ignore]
            public bool IgnoredField { get; set; }
        }

        private class EnumData
        {
            public enum Enum
            {
                NormalMember,

                [Deprecated("Some enum member reason")]
                DeprecatedMember,
            }
        }

        private class Implementation : ISomeInterface
        {
            public string FirstProperty { get; set; }

            [Ignore]
            public string SomeProperty { get; set; }

            public string SomeOtherProperty { get; set; }
        }

        private interface ISomeInterface
        {
            public string FirstProperty { get; set; }

            [Ignore]
            public string SomeProperty { get; set; }
        }

        private class ImplementationExternal : ISomeInterfaceExternal
        {
            public string FirstProperty { get; set; }

            public string SomeProperty { get; set; }

            public string SomeOtherProperty { get; set; }
        }

        private interface ISomeInterfaceExternal
        {
            public string FirstProperty { get; set; }

            public string SomeProperty { get; set; }
        }
    }
}
