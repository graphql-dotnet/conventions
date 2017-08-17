using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Tests;
using GraphQL.Conventions.Tests.Templates;

namespace GraphQL.Conventions.Tests.Execution
{
    public class UserContextWrapperTests : TestBase
    {
        [Test]
        public void Can_Create_Wrapper_Without_DataLoaderContextProvider_Support()
        {
            var wrapper = UserContextWrapper.Create(new UserContext(), new DependencyInjector());
            Assert.IsNotNull(wrapper);
            Assert.IsFalse(wrapper is IDataLoaderContextProvider);
        }

        [Test]
        public void Can_Create_Wrapper_With_DataLoaderContextProvider_Support()
        {
            var wrapper = UserContextWrapper.Create(new UserContextWithDataLoaderContextProvider(), new DependencyInjector());
            Assert.IsNotNull(wrapper);
            Assert.IsInstanceOfType(wrapper, typeof(IDataLoaderContextProvider));
        }

        [Test]
        public void Should_Forward_DataLoaderContextProvider_FetchData_Call()
        {
            var wrapper = UserContextWrapper.Create(new UserContextWithDataLoaderContextProvider(), new DependencyInjector());

            Assert.ThrowsException<FetchDataCalledException>(() => (wrapper as IDataLoaderContextProvider).FetchData(CancellationToken.None));
        }

        private class UserContext : IUserContext { }

        private class UserContextWithDataLoaderContextProvider : IUserContext, IDataLoaderContextProvider
        {
            public Task FetchData(CancellationToken token) => throw new FetchDataCalledException();
        }

        private class DependencyInjector : IDependencyInjector
        {
            public object Resolve(TypeInfo typeInfo) => null;
        }

        private class FetchDataCalledException : Exception { }
    }
}
