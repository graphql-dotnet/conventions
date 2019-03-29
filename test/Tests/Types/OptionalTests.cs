using System;
using GraphQL.Conventions.Tests.Templates;

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
    }
}
