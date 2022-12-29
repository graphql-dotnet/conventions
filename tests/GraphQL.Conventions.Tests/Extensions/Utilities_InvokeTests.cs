using System;
using System.Reflection;
using GraphQL.Conventions.Extensions;

// ReSharper disable UnusedType.Local
// ReSharper disable UnusedMember.Local

namespace Tests.Extensions
{
    public class UtilitiesInvokeTests
    {
        [Test]
        public void Static_Method_Returns_ValueType()
        {
            Assert.AreEqual(2, GetMethodInfo(nameof(StaticTest1)).InvokeEnhanced(null, null));
        }
        private static int StaticTest1() => 2;

        [Test]
        public void Static_Method_Returns_ObjectType()
        {
            Assert.AreEqual("hello", GetMethodInfo(nameof(StaticTest2)).InvokeEnhanced(null, null));
        }
        private static string StaticTest2() => "hello";

        [Test]
        public void Static_Method_Returns_Void()
        {
            _staticTest3Ran = false;
            Assert.IsNull(GetMethodInfo(nameof(StaticTest3)).InvokeEnhanced(null, null));
            Assert.IsTrue(_staticTest3Ran);
        }
        private static void StaticTest3() => _staticTest3Ran = true;
        private static bool _staticTest3Ran;

        [Test]
        public void Instance_Method_Returns_ValueType()
        {
            Assert.AreEqual(2, GetMethodInfo(nameof(InstanceTest1)).InvokeEnhanced(this, null));
        }
        private int InstanceTest1() => 2;

        [Test]
        public void Instance_Method_Returns_ObjectType()
        {
            Assert.AreEqual("hello", GetMethodInfo(nameof(InstanceTest2)).InvokeEnhanced(this, null));
        }
        private string InstanceTest2() => "hello";

        [Test]
        public void Instance_Method_Returns_Void()
        {
            _instanceTest3Ran = false;
            Assert.IsNull(GetMethodInfo(nameof(InstanceTest3)).InvokeEnhanced(this, null));
            Assert.IsTrue(_instanceTest3Ran);
        }
        private void InstanceTest3() => _instanceTest3Ran = true;
        private bool _instanceTest3Ran;

        [Test]
        public void Static_Method_WithParams()
        {
            Assert.AreEqual("Static Received 00000000-0000-0000-0000-000000000000 98 55", GetMethodInfo(nameof(StaticWithParams)).InvokeEnhanced(null, new object[] { Guid.Empty, new MyObject(98), 55 }));
        }
        private static string StaticWithParams(Guid guid, MyObject stream, int? value) => $"Static Received {guid} {stream} {value}";

        [Test]
        public void Instance_Method_WithParams()
        {
            Assert.AreEqual("Instance Received 00000000-0000-0000-0000-000000000000 99 56", GetMethodInfo(nameof(InstanceWithParams)).InvokeEnhanced(this, new object[] { Guid.Empty, new MyObject(99), 56 }));
        }
        private string InstanceWithParams(Guid guid, MyObject stream, int? value) => $"Instance Received {guid} {stream} {value}";

        [Test]
        public void Static_ThrowsExceptions()
        {
            try
            {
                GetMethodInfo(nameof(StaticThrows)).InvokeEnhanced(null, null);
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
                GetMethodInfo(nameof(InstanceThrows)).InvokeEnhanced(this, null);
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
            Assert.ThrowsException<ArgumentNullException>(() => Utilities.InvokeEnhanced(null, null, null));
        }

        [Test]
        public void Requires_Instance_For_Instance_Methods()
        {
            Assert.ThrowsException<ArgumentNullException>(() => GetMethodInfo(nameof(InstanceTest1)).InvokeEnhanced(null, null));
        }

        [Test]
        public void Ignores_Instance_For_Static_Methods()
        {
            Assert.AreEqual(2, GetMethodInfo(nameof(StaticTest1)).InvokeEnhanced(this, null));
        }

        [Test]
        public void Wrong_Arguments_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => GetMethodInfo(nameof(StaticTest1)).InvokeEnhanced(null, new object[] { 1 }));
        }

        [Test]
        public void RefArgument_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => GetMethodInfo(nameof(StaticRefTest1)).InvokeEnhanced(null, new object[] { 1 }));
        }
        // ReSharper disable once RedundantAssignment
        private static void StaticRefTest1(ref int value) => value = 2;

        [Test]
        public void Constructor_Exceptions_Are_Unwrapped()
        {
            var constructor = typeof(MyThrowingObject).GetConstructor(new Type[] { });
            try
            {
                constructor.InvokeEnhanced(null);
                throw new Exception("This code should not execute");
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        [Test]
        public void Constructor_Basic()
        {
            var obj = typeof(MyObject).GetConstructor(new Type[] { }).InvokeEnhanced(null);
            Assert.AreEqual("unknown", obj.ToString());
        }

        [Test]
        public void Constructor_ValueType()
        {
            var obj = typeof(MyObject).GetConstructor(new[] { typeof(int) }).InvokeEnhanced(new object[] { 1 });
            Assert.AreEqual("1", obj.ToString());
        }

        [Test]
        public void Constructor_ObjectType()
        {
            var obj = typeof(MyObject).GetConstructor(new[] { typeof(string) }).InvokeEnhanced(new object[] { "hello3" });
            Assert.AreEqual("hello3", obj.ToString());
        }

        [Test]
        public void Constructor_Multiple_Args()
        {
            var obj = typeof(MyObject).GetConstructor(new[] { typeof(Guid), typeof(MyObject) }).InvokeEnhanced(new object[] { Guid.Empty, new MyObject("hello5") });
            Assert.AreEqual("00000000-0000-0000-0000-000000000000 hello5", obj.ToString());
        }

        [Test]
        public void Constructor_ArgumentNull_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Utilities.InvokeEnhanced(null, null));
        }

        [Test]
        public void Constructor_Incorrect_Number_Of_Args_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => typeof(MyObject).GetConstructor(new Type[] { }).InvokeEnhanced(new object[] { 1 }));
        }

        [Test]
        public void Constructor_Wrong_Type_Of_Args_Throws()
        {
            Assert.ThrowsException<InvalidCastException>(() => typeof(MyObject).GetConstructor(new[] { typeof(int) }).InvokeEnhanced(new object[] { "test" }));
        }

        [Test]
        public void Constructor_Ref_Arg_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => typeof(MyObject).GetConstructors()[0].InvokeEnhanced(new object[] { 5 }));
        }

        private MethodInfo GetMethodInfo(string name) => typeof(UtilitiesInvokeTests).GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        private class MyObject
        {
            private readonly string ret;
            public MyObject()
            {
                ret = "unknown";
            }

            public MyObject(int value)
            {
                ret = value.ToString();
            }

            public MyObject(string value)
            {
                ret = value;
            }

            public MyObject(Guid value, MyObject obj)
            {
                ret = $"{value} {obj}";
            }

            public MyObject(out double value)
            {
                value = 5.0;
            }

            public override string ToString()
            {
                return ret;
            }
        }

        private class MyObjectRefC
        {
            public MyObjectRefC(out int value)
            {
                throw new InvalidTimeZoneException();
            }
        }

        private class MyThrowingObject
        {
            public MyThrowingObject()
            {
                throw new InvalidTimeZoneException();
            }
        }
    }
}
