using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Tests.Templates.Extensions
{
    static class TestExtensions
    {
        private static readonly Regex RegexStripBlankPrefixes = new Regex(@"^[ \t]+", RegexOptions.Multiline);

        public static void ShouldEqualWhenReformatted(this string actual, string expected)
        {
            expected = CleanMultilineText(expected);
            actual = CleanMultilineText(actual);
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldEqual(this string actual, string expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldEqual<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldNotEqual<T>(this T actual, T expected)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void ShouldBeGreaterThan(this int actual, int lowerBound)
        {
            Assert.IsTrue(actual > lowerBound);
        }

        public static void ShouldBeLessThan(this int actual, int upperBound)
        {
            Assert.IsTrue(actual < upperBound);
        }

        public static void ShouldBeNull<T>(this T actual)
        {
            Assert.IsNull(actual);
        }

        public static void ShouldNotBeNull<T>(this T actual)
        {
            Assert.IsNotNull(actual);
        }

        public static void ShouldBeEmpty(this string actual)
        {
            Assert.AreEqual(0, actual?.Length ?? 0);
        }

        public static void ShouldBeFalse(this bool actual, string message)
        {
            Assert.IsFalse(actual, message);
        }

        public static void ShouldBeTrue(this bool actual, string message)
        {
            Assert.IsTrue(actual, message);
        }

        public static void ShouldContain<T>(this IEnumerable<T> collection, T element)
        {
            Assert.IsTrue(collection.Contains(element));
        }

        public static void ShouldContain(this string str, string substring)
        {
            Assert.IsTrue(str.Contains(substring));
        }

        public static void ShouldContainWhenReformatted(this string str, string substring)
        {
            str = CleanMultilineText(str);
            substring = CleanMultilineText(substring);
            str.ShouldContain(substring);
        }

        public static void ShouldBeOfType<T>(this object actual)
        {
            Assert.IsInstanceOfType(actual, typeof(T));
        }

        public static void ShouldBeNamed(this GraphEntityInfo entity, string name)
        {
            entity.Name.ShouldEqual(name);
        }

        public static GraphFieldInfo ShouldHaveFieldWithName(this GraphTypeInfo type, string fieldName)
        {
            var field = type.Fields.FirstOrDefault(f => f.Name == fieldName);
            Assert.IsNotNull(field);
            return field;
        }

        public static void ShouldNotHaveFieldWithName(this GraphTypeInfo type, string fieldName)
        {
            var field = type.Fields.FirstOrDefault(f => f.Name == fieldName);
            Assert.IsNull(field);
        }

        public static GraphArgumentInfo ShouldHaveArgumentWithName(this GraphFieldInfo field, string argumentName)
        {
            var arg = field.Arguments.FirstOrDefault(a => a.Name == argumentName);
            Assert.IsNotNull(arg);
            return arg;
        }

        public static void ShouldNotHaveArgumentWithName(this GraphFieldInfo field, string argumentName)
        {
            var arg = field.Arguments.FirstOrDefault(a => a.Name == argumentName);
            Assert.IsNull(arg);
        }

        public static void ShouldNotBeDescribed(this GraphEntityInfo entity)
        {
            entity.Description.ShouldBeEmpty();
        }

        public static void AndWithoutDescription(this GraphEntityInfo entity)
        {
            entity.ShouldNotBeDescribed();
        }

        public static void ShouldHaveDescription(this GraphEntityInfo entity, string description)
        {
            entity.Description.ShouldEqual(description);
        }

        public static void AndWithDescription(this GraphEntityInfo entity, string description)
        {
            entity.ShouldHaveDescription(description);
        }

        public static void ShouldNotBeDeprecated(this GraphEntityInfo entity)
        {
            entity.IsDeprecated.ShouldEqual(false);
        }

        public static void AndWithoutDeprecationReason(this GraphEntityInfo entity)
        {
            entity.ShouldNotBeDeprecated();
        }

        public static void ShouldBeDeprecatedWithReason(this GraphEntityInfo entity, string reason)
        {
            entity.IsDeprecated.ShouldEqual(true);
            entity.DeprecationReason.ShouldEqual(reason);
        }

        public static void AndWithDeprecationReason(this GraphEntityInfo entity, string reason)
        {
            entity.ShouldBeDeprecatedWithReason(reason);
        }

        public static void AndFlaggedAsInjected(this GraphArgumentInfo entity)
        {
            entity.IsInjected.ShouldEqual(true);
        }

        public static void AndNotFlaggedAsInjected(this GraphArgumentInfo entity)
        {
            entity.IsInjected.ShouldEqual(false);
        }

        public static List<IAttribute> OnlyMetaDataFilters(this IEnumerable<IAttribute> attributes) =>
            attributes
                .Where(attr => attr is IMetaDataAttribute)
                .ToList();

        public static List<IAttribute> OnlyExecutionFilters(this IEnumerable<IAttribute> attributes) =>
            attributes
                .Where(attr => attr is IExecutionFilterAttribute)
                .ToList();

        public static List<IAttribute> ExcludeExecutionFilters(this IEnumerable<IAttribute> attributes) =>
            attributes
                .Where(attr => !(attr is IExecutionFilterAttribute))
                .ToList();

        public static void ShouldHaveFieldWithValue(this object result, params object[] pathAndValue)
        {
            var value = pathAndValue.Last();
            var path = pathAndValue.Take(pathAndValue.Length - 1).ToList();
            result.ShouldNotBeNull();
            var obj = result;
            foreach (var k in path.Take(path.Count - 1))
            {
                if (k is int)
                {
                    var array = obj as List<object>;
                    array.ShouldNotBeNull();
                    array.Count.ShouldBeGreaterThan((int)k);
                    obj = array[(int)k];
                    obj.ShouldNotBeNull();
                }
                else
                {
                    var dict = obj as Dictionary<string, object>;
                    dict.ShouldNotBeNull();
                    dict.Keys.ShouldContain(k);
                    obj = dict[k.ToString()];
                    obj.ShouldNotBeNull();
                }
            }
            var key = path.Last();
            if (key is int)
            {
                var array = obj as List<object>;
                array.ShouldNotBeNull();
                var output = array[(int)key];
                output.ShouldEqual(value);
            }
            else
            {
                var dict = obj as Dictionary<string, object>;
                dict.ShouldNotBeNull();
                var output = dict[key.ToString()];
                output.ShouldEqual(value);
            }
        }

        public static void ShouldHaveArrayFieldOfCount(this object result, params object[] pathAndValue)
        {
            var value = pathAndValue.Last() as int?;
            var path = pathAndValue.Take(pathAndValue.Length - 1).Select(p => p.ToString()).ToList();
            result.ShouldNotBeNull();
            var obj = result as Dictionary<string, object>;
            obj.ShouldNotBeNull();
            foreach (var key in path.Take(path.Count - 1))
            {
                obj.Keys.ShouldContain(key);
                obj = obj[key] as Dictionary<string, object>;
                obj.ShouldNotBeNull();
            }
            var array = obj[path.Last()] as List<object>;
            array.ShouldNotBeNull();
            array.Count.ShouldEqual(value ?? -1);
        }

        private static string CleanMultilineText(string value)
        {
            value = RegexStripBlankPrefixes.Replace(value, "").Trim();
            var lines = value
                .Trim()
                .Split('\n')
                .Select(line => line.Trim(' ', '\t', '\r', '\n').Replace('\\', '/'))
                .Where(line => !line.StartsWith("#"))
                .Where(line => !line.StartsWith("scalar "))
                .Where(line => line.Length > 0);
            return string.Join("\n", lines);
        }
    }
}