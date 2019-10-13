using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

namespace GraphQL.Conventions.Tests.Templates.Extensions
{
    static class ConstructionTestExtensions
    {
        public static void ShouldHaveQueries(this ISchema schema, int numberOfQueries)
        {
            if (numberOfQueries > 0)
            {
                Assert.IsNotNull(schema.Query);
            }
            if (schema.Query != null)
            {
                Assert.AreEqual(numberOfQueries, schema.Query.Fields.Count());
            }
        }

        public static void ShouldHaveMutations(this ISchema schema, int numberOfMutations)
        {
            if (numberOfMutations > 0)
            {
                Assert.IsNotNull(schema.Mutation);
            }
            if (schema.Mutation != null)
            {
                Assert.AreEqual(numberOfMutations, schema.Mutation.Fields.Count());
            }
        }

        public static void ShouldHaveSubscriptons(this ISchema schema, int numberOfSubscriptions)
        {
            if (numberOfSubscriptions > 0)
            {
                Assert.IsNotNull(schema.Subscription);
            }
            if (schema.Subscription != null)
            {
                Assert.AreEqual(numberOfSubscriptions, schema.Subscription.Fields.Count());
            }
        }

        public static void ShouldHaveFields(this IComplexGraphType type, int numberOfFields)
        {
            Assert.IsNotNull(type);
            Assert.AreEqual(numberOfFields, type.Fields.Count());
        }

        public static FieldType ShouldHaveFieldWithName(this IComplexGraphType type, string fieldName)
        {
            Assert.IsTrue(type.HasField(fieldName));
            return type.Fields.FirstOrDefault(field => field.Name == fieldName);
        }

        public static void ShouldNotHaveFieldWithName(this IComplexGraphType type, string fieldName)
        {
            Assert.IsFalse(type.HasField(fieldName));
        }

        public static void ShouldBeOfType<TType>(this Type type)
            where TType : GraphType
        {
            Assert.AreEqual(typeof(TType), type);
        }

        public static void ShouldBeOfNonNullableType<TType>(this Type type)
            where TType : GraphType
        {
            Assert.AreEqual(typeof(NonNullGraphType<TType>), type);
        }

        public static void ShouldBeOfType<TType>(this IGraphType type)
            where TType : GraphType
        {
            Assert.AreEqual(typeof(TType), type.GetType());
        }

        public static void ShouldBeOfNonNullableType<TType>(this IGraphType type)
            where TType : GraphType
        {
            Assert.AreEqual(typeof(NonNullGraphType<TType>), type.GetType());
        }

        public static void ShouldBeOfListType<TType>(this IGraphType type)
            where TType : GraphType
        {
            Assert.AreEqual(typeof(ListGraphType<TType>), type.GetType());
        }

        public static void ShouldBeOfNonNullableListType<TType>(this IGraphType type)
            where TType : GraphType
        {
            Assert.AreEqual(typeof(NonNullGraphType<ListGraphType<TType>>), type.GetType());
        }

        public static void OfType<TType>(this IFieldType field)
            where TType : GraphType
        {
            field.Type.ShouldBeOfType<TType>();
        }

        public static void OfNonNullableType<TType>(this IFieldType field)
            where TType : GraphType
        {
            field.Type.ShouldBeOfNonNullableType<TType>();
        }

        public static void ShouldContain(this IEnumerable<IGraphType> collection, string name)
        {
            Assert.IsTrue(collection.Any(type => type.Name == name));
        }

        public static QueryArgument ShouldHaveArgument(this IFieldType fieldType, string name)
        {
            var arg = fieldType.Arguments.FirstOrDefault(argument => argument.Name == name);
            Assert.IsNotNull(arg);
            return arg;
        }

        public static void ShouldHaveNoErrors(this ExecutionResult result)
        {
            result.ShouldNotBeNull();
            if ((result?.Errors?.Count ?? 0) > 0)
            {
                var messages = string.Join("\n---\n", result.Errors.Select(ExceptionToString));
                throw new Exception(
                    $"Expected no errors, but got {result.Errors.Count}:\n\n{messages}\n");
            }
        }

        public static void ShouldHaveErrors(this ExecutionResult result, int count)
        {
            result.ShouldNotBeNull();
            if ((result?.Errors?.Count ?? 0) != count)
            {
                var messages = result?.Errors != null
                    ? string.Join("\n---\n", result.Errors.Select(ExceptionToString))
                    : "(none)";
                throw new Exception(
                    $"Expected {count} errors, but got {result?.Errors.Count}:\n\n{messages}\n");
            }
        }

        private static string ExceptionToString(ExecutionError error)
        {
            var locationStrings = error.Locations?.Select(loc => $"(Line: {loc.Line}, Column: {loc.Column})");
            var locations = locationStrings != null ? string.Join(", ", locationStrings) : string.Empty;
            return $"{error.Message}\nLocations: {locations}\n{error}";
        }
    }
}