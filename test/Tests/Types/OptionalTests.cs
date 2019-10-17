using System;
using System.Reflection;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Tests.Types
{
    public class OptionalTests : TestBase
    {
        [Test]
        public void Can_Define_String()
        {
            Optional<string>.ValidateType();
        }

        [Test]
        public void Cannot_Define_NonNullable_Int()
        {
            Assert.ThrowsException<TypeInitializationException>(() => Optional<int>.ValidateType());
        }

         [Test]
        public void Can_Define_Nullable_Int()
        {
            Optional<int?>.ValidateType();
        }

       [Test]
        public void Cannot_Define_NonNullable_DateTime()
        {
            Assert.ThrowsException<TypeInitializationException>(() => Optional<DateTime>.ValidateType());
        }

        [Test]
        public void Can_Define_Nullable_DateTime()
        {
            Optional<DateTime?>.ValidateType();
        }

        [Test]
        public void Cannot_Define_NonNullable_DateTimeOffset()
        {
            Assert.ThrowsException<TypeInitializationException>(() => Optional<DateTimeOffset>.ValidateType());
        }

        [Test]
        public void Can_Define_Nullable_DateTimeOffset()
        {
            Optional<DateTimeOffset?>.ValidateType();
        }

        [Test]
        public void Cannot_Define_NonNullable_TimeSpan()
        {
            Assert.ThrowsException<TypeInitializationException>(() => Optional<TimeSpan>.ValidateType());
        }

        [Test]
        public void Can_Define_Nullable_TimeSpan()
        {
            Optional<TimeSpan?>.ValidateType();
        }

        [Test]
        public void Cannot_Define_NonNullable_Boolean()
        {
            Assert.ThrowsException<TypeInitializationException>(() => Optional<bool>.ValidateType());
        }

        [Test]
        public void Can_Define_Nullable_Boolean()
        {
            Optional<bool?>.ValidateType();
        }

        [Test]
        public void Cannot_Define_NonNullable_Float()
        {
            Assert.ThrowsException<TypeInitializationException>(() => Optional<float>.ValidateType());
        }

        [Test]
        public void Can_Define_Nullable_Float()
        {
            Optional<float?>.ValidateType();
        }

        [Test]
        public void Cannot_Define_NonNullable_Guid()
        {
            Assert.ThrowsException<TypeInitializationException>(() => Optional<Guid>.ValidateType());
        }

        [Test]
        public void Can_Define_Nullable_Guid()
        {
            Optional<Guid?>.ValidateType();
        }

        [Test]
        public void Can_Define_Uri()
        {
            Optional<Uri>.ValidateType();
        }

        [Test]
        public void Can_Define_Url()
        {
            Optional<Url>.ValidateType();
        }

        [Test]
        public void Can_Construct_WithNull()
        {
            var typeResolver = new TypeResolver();
            var typeInfo = typeof(Optional<int?>).GetTypeInfo();
            var graphTypeInfo = new GraphTypeInfo(typeResolver, typeInfo);
            var optional = (Optional<int?>) Optional.Construct(graphTypeInfo, null, true);
            Assert.IsNotNull(optional);
            Assert.IsNull(optional.Value);
        }

        [Test]
        public void Can_Construct_WithValueType()
        {
            int value = 2;
            var typeResolver = new TypeResolver();
            var typeInfo = typeof(Optional<int?>).GetTypeInfo();
            var graphTypeInfo = new GraphTypeInfo(typeResolver, typeInfo);
            var optional = (Optional<int?>)Optional.Construct(graphTypeInfo, value, true);
            Assert.IsNotNull(optional);
            Assert.IsNotNull(optional.Value);
            Assert.AreEqual(value, optional.Value);
        }

        [Test]
        public void Can_Construct_WithNullable()
        {
            int? value = 2;
            var typeResolver = new TypeResolver();
            var typeInfo = typeof(Optional<int?>).GetTypeInfo();
            var graphTypeInfo = new GraphTypeInfo(typeResolver, typeInfo);
            var optional = (Optional<int?>)Optional.Construct(graphTypeInfo, value, true);
            Assert.IsNotNull(optional);
            Assert.IsNotNull(optional.Value);
            Assert.AreEqual(value, optional.Value);
        }
    }
}
