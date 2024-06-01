using System;
using System.Collections;
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
        [MemberData(nameof(IsEnumerableGraphType_Should_Return_True_For_Common_Collection_Types_Data))]
        public void IsEnumerableGraphType_Should_Return_True_For_Common_Collection_Types(Type type)
        {
            Assert.IsTrue(type.GetTypeInfo().IsEnumerableGraphType());
        }

        public static TheoryData<Type> IsEnumerableGraphType_Should_Return_True_For_Common_Collection_Types_Data() => new()
        {
            typeof(IEnumerable<>),
            typeof(ConcurrentQueue<>),
            typeof(HashSet<>),
            typeof(int[]),
            typeof(List<>),
            typeof(IList<>),
            typeof(IReadOnlyList<>),
            typeof(IReadOnlyCollection<>),
        };

        [Theory]
        [MemberData(nameof(IsEnumerableGraphType_Should_Return_False_For_Common_Dictionary_Types_Data))]
        public void IsEnumerableGraphType_Should_Return_False_For_Common_Dictionary_Types(Type type)
        {
            Assert.IsFalse(type.GetTypeInfo().IsEnumerableGraphType());
        }

        public static TheoryData<Type> IsEnumerableGraphType_Should_Return_False_For_Common_Dictionary_Types_Data() => new()
        {
            typeof(IDictionary<,>),
            typeof(IDictionary),
        };

        [Theory]
        [MemberData(nameof(GetImplementationInterface_WithoutFuse_AcquireSpecifiedInterfaceOnly_Data))]
        public void GetImplementationInterface_WithoutFuse_AcquireSpecifiedInterfaceOnly(Type type, Type assignableInterface)
            => Assert.IsTrue(type.IsImplementingInterface(assignableInterface, false));

        public static TheoryData<Type, Type> GetImplementationInterface_WithoutFuse_AcquireSpecifiedInterfaceOnly_Data() =>
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
        [MemberData(nameof(GetImplementationInterface_WithFuse_AcquireSpecifiedInterfaceOnly_Data))]
        public void GetImplementationInterface_WithFuse_AcquireSpecifiedInterfaceOnly(Type type, Type assignableInterface) =>
            Assert.IsTrue(type.IsImplementingInterface(assignableInterface));

        public static TheoryData<Type, Type> GetImplementationInterface_WithFuse_AcquireSpecifiedInterfaceOnly_Data() =>
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
