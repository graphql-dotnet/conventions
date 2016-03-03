using System.Linq;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Types;
using Xunit;
using Extended = GraphQL.Conventions.Adapters.Types;

namespace GraphQL.Conventions.Tests.Adapters
{
    public class TypeConstructionTests : ConstructionTestBase
    {
        [Fact]
        public void Can_Derive_Output_Type()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<OutputType>());
            Assert.IsAssignableFrom(typeof(ObjectGraphType), type);
        }

        [Fact]
        public void Can_Derive_Input_Type()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<InputType>());
            Assert.IsAssignableFrom(typeof(InputObjectGraphType), type);
        }

        [Fact]
        public void Can_Derive_Type_Without_Description()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<TypeWithoutDescription>());
            type.Name.ShouldEqual(nameof(TypeWithoutDescription));
            type.Description.ShouldBeEmpty();
        }

        [Fact]
        public void Can_Derive_Type_With_Description()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<TypeWithDescription>());
            type.Name.ShouldEqual(nameof(TypeWithDescription));
            type.Description.ShouldEqual("Some Description");
        }

        [Fact]
        public void Can_Derive_Type_With_Overridden_Name()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<TypeWithOverriddenName>());
            type.Name.ShouldEqual("Foo");
        }

        [Fact]
        public void Can_Derive_Type_Without_Deprecation_Reason()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<TypeWithoutDeprecationReason>());
            type.DeprecationReason.ShouldBeEmpty();
        }

        [Fact]
        public void Can_Derive_Type_With_Deprecation_Reason()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<TypeWithDeprecationReason>());
            type.DeprecationReason.ShouldEqual("Some Deprecation Reason");
        }

        [Fact]
        public void Can_Derive_Interface()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<IInterface>()) as IComplexGraphType;
            Assert.IsAssignableFrom(typeof(IInterfaceGraphType), type);
            type.ShouldHaveFields(1);
            type.ShouldHaveFieldWithName("interfaceField");
        }

        [Fact]
        public void Can_Derive_Type_Implementing_Interface()
        {
            var typeResolver = new TypeResolver();
            var type = Type<ObjectGraphType>(typeResolver.DeriveType<TypeImplementingInterfaces>());

            type.ShouldHaveFields(2);
            type.ShouldHaveFieldWithName("interfaceField");
            type.ShouldHaveFieldWithName("typeField");

            type.Interfaces.Count().ShouldEqual(1);
            type.Interfaces.ShouldContain(typeof(Extended.InterfaceGraphType<IInterface>));

            var iface = Type<IInterfaceGraphType>(typeResolver.DeriveType<IInterface>());
            iface.PossibleTypes.ShouldContain(nameof(TypeImplementingInterfaces));
        }

        class OutputType
        {
        }

        [InputType]
        class InputType
        {
        }

        class TypeWithoutDescription
        {
        }

        [Description("Some Description")]
        class TypeWithDescription
        {
        }

        [Name("Foo")]
        class TypeWithOverriddenName
        {
        }

        class TypeWithoutDeprecationReason
        {
        }

        [Deprecated("Some Deprecation Reason")]
        class TypeWithDeprecationReason
        {
        }

        interface IInterface
        {
            int InterfaceField { get; }
        }

        class TypeImplementingInterfaces : IInterface
        {
            public int InterfaceField => 1;

            public int TypeField => 2;
        }
    }
}