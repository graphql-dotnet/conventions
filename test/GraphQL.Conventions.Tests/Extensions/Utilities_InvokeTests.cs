using GraphQL.Conventions.Extensions;
using GraphQL.Conventions.Tests;
using System;
using System.Reflection;

namespace Tests.Extensions
{
    public class Utilities_InvokeTests
    {
        [Test]
        public void Static_Method_Returns_ValueType()
        {
            Assert.AreEqual(2, GetMethodInfo(nameof(StaticTest1)).InvokeEnhanced(null));
        }
        private static int StaticTest1() => 2;

        [Test]
        public void Static_Method_Returns_ObjectType()
        {
            Assert.AreEqual("hello", GetMethodInfo(nameof(StaticTest2)).InvokeEnhanced(null));
        }
        private static string StaticTest2() => "hello";

        [Test]
        public void Static_Method_Returns_Void()
        {
            StaticTest3Ran = false;
            Assert.IsNull(GetMethodInfo(nameof(StaticTest3)).InvokeEnhanced(null));
            Assert.IsTrue(StaticTest3Ran);
        }
        private static void StaticTest3() => StaticTest3Ran = true;
        private static bool StaticTest3Ran;

        [Test]
        public void Instance_Method_Returns_ValueType()
        {
            Assert.AreEqual(2, GetMethodInfo(nameof(InstanceTest1)).InvokeEnhanced(this));
        }
        private int InstanceTest1() => 2;

        [Test]
        public void Instance_Method_Returns_ObjectType()
        {
            Assert.AreEqual("hello", GetMethodInfo(nameof(InstanceTest2)).InvokeEnhanced(this));
        }
        private string InstanceTest2() => "hello";

        [Test]
        public void Instance_Method_Returns_Void()
        {
            InstanceTest3Ran = false;
            Assert.IsNull(GetMethodInfo(nameof(InstanceTest3)).InvokeEnhanced(this));
            Assert.IsTrue(InstanceTest3Ran);
        }
        private void InstanceTest3() => InstanceTest3Ran = true;
        private bool InstanceTest3Ran;

        [Test]
        public void Static_Method_WithParams()
        {
            Assert.AreEqual("Static Received 00000000-0000-0000-0000-000000000000 98 55", GetMethodInfo(nameof(StaticWithParams)).InvokeEnhanced(null, Guid.Empty, new MyObject(98), 55));
        }
        private static string StaticWithParams(Guid guid, MyObject stream, int? value) => $"Static Received {guid} {stream} {value}";

        [Test]
        public void Instance_Method_WithParams()
        {
            Assert.AreEqual("Instance Received 00000000-0000-0000-0000-000000000000 99 56", GetMethodInfo(nameof(InstanceWithParams)).InvokeEnhanced(this, Guid.Empty, new MyObject(99), 56));
        }
        private string InstanceWithParams(Guid guid, MyObject stream, int? value) => $"Instance Received {guid} {stream} {value}";

        [Test]
        public void Static_ThrowsExceptions()
        {
            try
            {
                GetMethodInfo(nameof(StaticThrows)).InvokeEnhanced(null);
            }
            catch (Exception e)
            {
                Assert.AreEqual("test exception", e.Message);
            }
        }
        private static void StaticThrows() => throw new Exception("test exception");

        [Test]
        public void Instance_ThrowsExceptions()
        {
            try
            {
                GetMethodInfo(nameof(InstanceThrows)).InvokeEnhanced(this);
            }
            catch (Exception e)
            {
                Assert.AreEqual("test exception2", e.Message);
            }
        }
        private void InstanceThrows() => throw new Exception("test exception2");

        [Test]
        public void Requires_MethodInfo()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Utilities.InvokeEnhanced((MethodInfo)null, null));
        }

        [Test]
        public void Requires_Instance_For_Instance_Methods()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Utilities.InvokeEnhanced(GetMethodInfo(nameof(InstanceTest1)), null));
        }

        [Test]
        public void Ignores_Instance_For_Static_Methods()
        {
            Assert.AreEqual(2, GetMethodInfo(nameof(StaticTest1)).InvokeEnhanced(this));
        }

        [Test]
        public void Wrong_Arguments_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => GetMethodInfo(nameof(StaticTest1)).InvokeEnhanced(null, 1));
        }

        [Test]
        public void RefArgument_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => GetMethodInfo(nameof(StaticRefTest1)).InvokeEnhanced(null, new object[] { 1 }));
        }
        private static void StaticRefTest1(ref int value) => value = 2;

        private MethodInfo GetMethodInfo(string name) => typeof(Utilities_InvokeTests).GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        private class MyObject
        {
            private int _int;
            public MyObject(int value) => _int = value;
            public override string ToString()
            {
                return _int.ToString();
            }
        }

    }
}
