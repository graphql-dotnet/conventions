using System;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution;
using Xunit;

namespace GraphQL.Conventions.Tests.Templates
{
    public abstract class TestBase
    {
        protected void ShouldThrow<TException>(Action action)
            where TException : Exception
        {
            Assert.Throws(typeof(TException), action);
        }

        protected GraphTypeInfo TypeInfo<TType>()
        {
            var typeResolver = new TypeResolver();
            return typeResolver.DeriveType<TType>();
        }
    }
}