using System;
using System.Linq;
using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Types;
using Tests.Templates;
using Tests.Templates.Extensions;
using Extended = GraphQL.Conventions.Adapters.Types;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Tests.Adapters
{
    public class TypeConstructionTests : ConstructionTestBase
    {
        [Test]
        public void Can_Derive_Output_Type()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<OutputType>());
            Assert.IsInstanceOfType(type, typeof(ObjectGraphType));
        }

        [Test]
        public void Can_Derive_Input_Type()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<InputType>());
            Assert.IsInstanceOfType(type, typeof(InputObjectGraphType));
        }

        [Test]
        public void Can_Derive_Type_Without_Description()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<TypeWithoutDescription>());
            type.Name.ShouldEqual(nameof(TypeWithoutDescription));
            type.Description.ShouldBeEmpty();
        }

        [Test]
        public void Can_Derive_Type_With_Description()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<TypeWithDescription>());
            type.Name.ShouldEqual(nameof(TypeWithDescription));
            type.Description.ShouldEqual("Some Description");
        }

        [Test]
        public void Can_Derive_Type_With_Overridden_Name()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<TypeWithOverriddenName>());
            type.Name.ShouldEqual("Foo");
        }

        [Test]
        public void Can_Derive_Type_Without_Deprecation_Reason()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<TypeWithoutDeprecationReason>());
            type.DeprecationReason.ShouldBeEmpty();
        }

        [Test]
        public void Can_Derive_Type_With_Deprecation_Reason()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<TypeWithDeprecationReason>());
            type.DeprecationReason.ShouldEqual("Some Deprecation Reason");
        }

        [Test]
        public void Can_Derive_Interface()
        {
            var typeResolver = new TypeResolver();
            var type = Type(typeResolver.DeriveType<IInterface>()) as IComplexGraphType;
            Assert.IsInstanceOfType(type, typeof(IInterfaceGraphType));
            type.ShouldHaveFields(1);
            type.ShouldHaveFieldWithName("interfaceField");
        }

        [Test]
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

        [Test]
        public void Can_Derive_Type_Implementing_Multiple_Interfaces()
        {
            var typeResolver = new TypeResolver();
            var type = Type<ObjectGraphType>(typeResolver.DeriveType<TypeImplementingTwoInterfaces>());

            type.ShouldHaveFields(2);
            type.ShouldHaveFieldWithName("field1");
            type.ShouldHaveFieldWithName("field2");

            type.Interfaces.Count().ShouldEqual(2);
            type.Interfaces.ShouldContain(typeof(Extended.InterfaceGraphType<IInterface1>));
            type.Interfaces.ShouldContain(typeof(Extended.InterfaceGraphType<IInterface2>));

            var iface1 = Type<IInterfaceGraphType>(typeResolver.DeriveType<IInterface1>());
            iface1.PossibleTypes.ShouldContain(nameof(TypeImplementingTwoInterfaces));
            iface1.Name.ShouldEqual("IInterface1");

            var iface2 = Type<IInterfaceGraphType>(typeResolver.DeriveType<IInterface2>());
            iface2.PossibleTypes.ShouldContain(nameof(TypeImplementingTwoInterfaces));
            iface2.Name.ShouldEqual("IInterface2");
        }

        [Test]
        public void Bug190_Work_For_Nested_Nodes()
        {
            var typeInfo = TypeInfo<NonNull<Bug190Query.Node1>>();
            var _ = Type(typeInfo);
        }

        [Test]
        public void Bug190_Discover_Possible_Types_From_Lists()
        {
            var schema = GraphQLEngine.New<Bug190Query_2>().GetSchema();
            var type = new SchemaTypes(schema, new DefaultServiceProvider())[nameof(Bug190Query_2.TestImpl)];
            type.ShouldNotBeNull();
        }

        class Bug190Query
        {
            public NonNull<Node1> Test() => throw new NotImplementedException();

            public class Node1 : INode
            {
                public Id Id => throw new NotImplementedException();

                public Node2[] Test2() => throw new NotImplementedException();
            }

            public class Node2 : INode
            {
                public Id Id => throw new NotImplementedException();
            }
        }

        class Bug190Query_2
        {
            public ITest[] Field { get; }

            public interface ITest
            {
                int Test { get; }
            }
            public class TestImpl: ITest
            {
                public int Test { get; }
            }
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

        interface IInterface1
        {
            int Field1 { get; }
        }

        interface IInterface2
        {
            int Field2 { get; }
        }

        class TypeImplementingTwoInterfaces : IInterface1, IInterface2
        {
            public int Field1 => 1;

            public int Field2 => 2;
        }
    }
}
