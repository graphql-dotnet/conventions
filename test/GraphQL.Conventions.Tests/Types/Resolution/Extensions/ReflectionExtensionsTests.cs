using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using GraphQL.Conventions.Types.Resolution.Extensions;
using Xunit;

namespace Tests.Types.Resolution.Extensions
{
    public class ReflectionExtensionsTests
    {
        [Theory]
        [MemberData(nameof(IsEnumerableGraphType_Should_ReturnTrue_For_Common_Collection_Types__Data))]
        public void IsEnumerableGraphType_Should_ReturnTrue_For_Common_Collection_Types(Type type)
        {
            Assert.IsTrue(type.GetTypeInfo().IsEnumerableGraphType());
        }

        public static TheoryData<Type> IsEnumerableGraphType_Should_ReturnTrue_For_Common_Collection_Types__Data() => new()
        {
            typeof(IEnumerable<>),
            typeof(ConcurrentQueue<>),
            typeof(HashSet<>),
            typeof(int[]),
            typeof(List<>),
            typeof(IList<>),
            typeof(IReadOnlyList<>),
            typeof(IReadOnlyCollection<>)
        };

        [Theory]
        [MemberData(nameof(GetImplementInterface_WithoutFuse_AcquireSpecifyInterfaceOnly__Data))]
        public void GetImplementInterface_WithoutFuse_AcquireSpecifyInterfaceOnly(Type type, Type assignableInterface)
            => Assert.IsTrue(type.ImplementInterface(assignableInterface, false));

        public static TheoryData<Type, Type> GetImplementInterface_WithoutFuse_AcquireSpecifyInterfaceOnly__Data() =>
            new()
            {
                { typeof(ITestInterface1<int>), typeof(ITestInterface1<int>) },
                { typeof(ITestInterface2<int>), typeof(ITestInterface1<int>) },
                { typeof(ITestInterface2<int>), typeof(ITestInterface2<int>) },
                { typeof(TestClass11<int>), typeof(ITestInterface1<int>) },
                { typeof(TestClass12), typeof(ITestInterface1<string>) },
                { typeof(TestClass12), typeof(ITestInterface2<string>) },
                { typeof(TestClass2), typeof(ITestInterface1<int>) },
                { typeof(TestClass2), typeof(ITestInterface1<string>) },
                { typeof(TestClass2), typeof(ITestInterface2<string>) },
            };

        [Theory]
        [MemberData(nameof(GetImplementInterface_WithFuse_AcquireSpecifyInterfaceOnly__Data))]
        public void GetImplementInterface_WithFuse_AcquireSpecifyInterfaceOnly(Type type, Type assignableInterface) =>
            Assert.IsTrue(type.ImplementInterface(assignableInterface));

        public static TheoryData<Type, Type> GetImplementInterface_WithFuse_AcquireSpecifyInterfaceOnly__Data() =>
            new()
            {
                { typeof(ITestInterface1<int>), typeof(ITestInterface1<>) },
                { typeof(ITestInterface2<int>), typeof(ITestInterface1<>) },
                { typeof(ITestInterface2<int>), typeof(ITestInterface2<>) },
                { typeof(TestClass11<int>), typeof(ITestInterface1<>) },
                //
                { typeof(ITestInterface1<string>), typeof(ITestInterface1<>) },
                { typeof(ITestInterface2<string>), typeof(ITestInterface1<>) },
                { typeof(ITestInterface2<string>), typeof(ITestInterface2<>) },
                { typeof(TestClass11<string>), typeof(ITestInterface1<>) },
                //
                { typeof(TestClass12), typeof(ITestInterface1<>) },
                { typeof(TestClass12), typeof(ITestInterface2<>) },
                { typeof(TestClass2), typeof(ITestInterface1<>) },
                { typeof(TestClass2), typeof(ITestInterface2<>) },
            };

        private interface ITestInterface1<out T>
        {
            // ReSharper disable once UnusedMember.Global
            T Item => throw new NotImplementedException();
        }

        private interface ITestInterface2<out T> : ITestInterface1<T>
        {
        }

        private class TestClass11<T> : ITestInterface1<T>
        {
        }

        private class TestClass12 : ITestInterface2<string>
        {
        }

        private class TestClass2 : ITestInterface2<string>, ITestInterface1<int>
        {
        }
    }
}
