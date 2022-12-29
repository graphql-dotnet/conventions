using System;
using System.Collections.Generic;
using System.Reflection;
using GraphQL.Conventions;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Attributes.Collectors;
using GraphQL.Conventions.Attributes.MetaData;
using Tests.Templates;
using Tests.Templates.Extensions;

namespace Tests.Attributes.Collectors
{
    public class AttributeCollectorTests : TestBase
    {
        private readonly AttributeCollector<IAttribute> _collector = new AttributeCollector<IAttribute>();

        [Test]
        public void Can_Find_Default_Attributes()
        {
            var attributes = _collector
                .CollectAttributes(typeof(TypeWithDefaultAttribute).GetTypeInfo())
                .ExcludeExecutionFilters();
            attributes.Count.ShouldEqual(2);
            attributes[0].GetType().ShouldEqual(typeof(CoreAttribute));
            attributes[1].GetType().ShouldEqual(typeof(NameAttribute));
        }

        [Test]
        public void Can_Find_Explicit_Attributes()
        {
            var attributes = _collector
                .CollectAttributes(typeof(TypeWithExplicitAttribute).GetTypeInfo())
                .ExcludeExecutionFilters();
            attributes.Count.ShouldEqual(3);
            attributes[0].GetType().ShouldEqual(typeof(CoreAttribute));
            attributes[1].GetType().ShouldEqual(typeof(NameAttribute));
            attributes[2].GetType().ShouldEqual(typeof(TestAttribute));
            ((TestAttribute)attributes[2]).Identifier.ShouldEqual(1);
        }

        [Test]
        public void Can_Find_Explicit_Attributes_On_Derived_Types()
        {
            var attributes = _collector
                .CollectAttributes(typeof(DerivedTypeWithExplicitAttribute).GetTypeInfo())
                .ExcludeExecutionFilters();
            attributes.Count.ShouldEqual(5);
            attributes[0].GetType().ShouldEqual(typeof(CoreAttribute));
            attributes[1].GetType().ShouldEqual(typeof(NameAttribute));
            attributes[2].GetType().ShouldEqual(typeof(TestAttribute));
            ((TestAttribute)attributes[2]).Identifier.ShouldEqual(1);
            attributes[3].GetType().ShouldEqual(typeof(TestAttribute));
            ((TestAttribute)attributes[3]).Identifier.ShouldEqual(2);
            attributes[4].GetType().ShouldEqual(typeof(TestAttribute));
            ((TestAttribute)attributes[4]).Identifier.ShouldEqual(3);
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        private class TestAttribute : Attribute, IAttribute
        {
            public List<IAttribute> AssociatedAttributes => new List<IAttribute>();

            public int ApplicationOrder => 0;

            public AttributeApplicationPhase Phase => AttributeApplicationPhase.MetaDataDerivation;

            public int Identifier { get; set; }
        }

        private class TypeWithDefaultAttribute
        {
        }

        [Test(Identifier = 1)]
        private class TypeWithExplicitAttribute
        {
        }

        [Test(Identifier = 3)]
        [Test(Identifier = 2)]
        private class DerivedTypeWithExplicitAttribute : TypeWithExplicitAttribute
        {
        }
    }
}
