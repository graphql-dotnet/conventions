using System;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Tests.Templates
{
    public abstract class TestBase
    {
        protected void ShouldThrow<TException>(Action action)
            where TException : Exception
        {
            Assert.ThrowsException<TException>(action);
        }

        protected GraphTypeInfo TypeInfo<TType>()
        {
            var typeResolver = new TypeResolver();
            return typeResolver.DeriveType<TType>();
        }
    }
}