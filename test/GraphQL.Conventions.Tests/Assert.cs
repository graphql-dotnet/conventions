using System;

namespace GraphQL.Conventions.Tests
{
    public static class Assert
    {
        public static void AreEqual(object expected, object actual) => Xunit.Assert.Equal(expected, actual);

        public static void AreNotEqual(object expected, object actual) => Xunit.Assert.NotEqual(expected, actual);

        public static void IsInstanceOfType(object actual, Type expectedRootType) => Xunit.Assert.IsAssignableFrom(expectedRootType, actual);

        public static void ThrowsException<T>(Action action) where T : Exception => Xunit.Assert.Throws<T>(action);

        public static void IsNull(object value) => AreEqual(null, value);

        public static void IsNotNull(object value) => AreNotEqual(null, value);

        public static void IsTrue(object actual, string message = null) => AreEqual(true, actual);

        public static void IsFalse(object actual, string message = null) => AreEqual(false, actual);
    }
}