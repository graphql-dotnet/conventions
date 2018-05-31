using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Adapters
{
    public class ArgumentResolutionTests : TestBase
    {
        [Test]
        public async Task Can_Resolve_Argument_Using_Dependency_Injection()
        {
            var result = await ExecuteQuery(@"{ dependencyInjectionField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("dependencyInjectionField", "Injection");
        }

        [Test]
        public async Task Can_Resolve_Value_Type_Argument()
        {
            var result = await ExecuteQuery(
                @"{ valueTypeField(valueTypeArg: 123) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("valueTypeField", 123);

            result = await ExecuteQuery(
                @"query Test($arg: Int!) { valueTypeField(valueTypeArg: $arg) }",
                new Dictionary<string, object> { { "arg", (int?)123 } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("valueTypeField", 123);
        }

        [Test]
        public async Task Can_Resolve_Nullable_Value_Type_Argument()
        {
            var result = await ExecuteQuery(
                @"{ nullableValueTypeField(nullableValueTypeArg: 123) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nullableValueTypeField", 123);

            result = await ExecuteQuery(
                @"{ nullableValueTypeField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nullableValueTypeField", null);

            result = await ExecuteQuery(
                @"query Test($arg: Int) { nullableValueTypeField(nullableValueTypeArg: $arg) }",
                new Dictionary<string, object> { { "arg", 123 } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nullableValueTypeField", 123);

            result = await ExecuteQuery(
                @"query Test($arg: Int) { nullableValueTypeField(nullableValueTypeArg: $arg) }",
                new Dictionary<string, object> { { "arg", null } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nullableValueTypeField", null);
        }

        [Test]
        public async Task Can_Resolve_Nullable_Primitive_Argument()
        {
            var result = await ExecuteQuery(
                @"{ nullablePrimitiveField(nullablePrimitiveArg: ""Foo"") }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nullablePrimitiveField", "Foo");

            result = await ExecuteQuery(
                @"query Test($arg: String) { nullablePrimitiveField(nullablePrimitiveArg: $arg) }",
                new Dictionary<string, object> { { "arg", "Foo" } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nullablePrimitiveField", "Foo");
        }

        [Test]
        public async Task Can_Resolve_NonNullable_Primitive_Argument()
        {
            var result = await ExecuteQuery(
                @"{ nonNullablePrimitiveField(nonNullablePrimitiveArg: ""Foo"") }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullablePrimitiveField", "Foo");

            result = await ExecuteQuery(
                @"query Test($arg: String!) { nonNullablePrimitiveField(nonNullablePrimitiveArg: $arg) }",
                new Dictionary<string, object> { { "arg", "Foo" } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullablePrimitiveField", "Foo");
        }

        [Test]
        public async Task Can_Resolve_Nullable_Identifier_Argument()
        {
            var id = Id.New<Dependency>("12345");
            var result = await ExecuteQuery(
                @"{ nullableIdField(idArg: """ + id.ToString() +  @""") }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nullableIdField", id.IdentifierForType<Dependency>());

            result = await ExecuteQuery(
                @"query Test($arg: ID) { nullableIdField(idArg: $arg) }",
                new Dictionary<string, object> { { "arg", id.ToString() } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nullableIdField", id.IdentifierForType<Dependency>());
        }

        [Test]
        public async Task Can_Resolve_NonNullable_Identifier_Argument()
        {
            var id = Id.New<Dependency>("12345");
            var result = await ExecuteQuery(
                @"{ nonNullableIdField(idArg: """ + id.ToString() +  @""") }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullableIdField", id.IdentifierForType<Dependency>());

            result = await ExecuteQuery(
                @"query Test($arg: ID!) { nonNullableIdField(idArg: $arg) }",
                new Dictionary<string, object> { { "arg", id.ToString() } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullableIdField", id.IdentifierForType<Dependency>());
        }

        [Test]
        public async Task Can_Resolve_Nullable_Argument_With_Default_Value()
        {
            var result = await ExecuteQuery(
                @"{ defaultNullableArgumentValueField(arg: 123) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("defaultNullableArgumentValueField", 123);

            result = await ExecuteQuery(
                @"{ defaultNullableArgumentValueField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("defaultNullableArgumentValueField", 999);

            result = await ExecuteQuery(
                @"query Test($arg: Int) { defaultNullableArgumentValueField(arg: $arg) }",
                new Dictionary<string, object> { { "arg", 123 } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("defaultNullableArgumentValueField", 123);

            result = await ExecuteQuery(
                @"query Test($arg: Int) { defaultNullableArgumentValueField(arg: $arg) }",
                new Dictionary<string, object> { { "arg", null } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("defaultNullableArgumentValueField", 999);
        }

        [Test]
        public async Task Can_Resolve_NonNullable_Argument_With_Default_Value()
        {
            var result = await ExecuteQuery(
                @"{ defaultNonNullableArgumentValueField(arg: 123) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("defaultNonNullableArgumentValueField", 123);

            result = await ExecuteQuery(
                @"query Test($arg: Int!) { defaultNonNullableArgumentValueField(arg: $arg) }",
                new Dictionary<string, object> { { "arg", 123 } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("defaultNonNullableArgumentValueField", 123);
        }

        [Test]
        public async Task Can_Resolve_Nullable_Enum_Argument()
        {
            var result = await ExecuteQuery(
                @"{ enumField: nullableEnumField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", null);

            result = await ExecuteQuery(
                @"query Test($arg: TestEnum) { enumField: nullableEnumField(enumArg: $arg) }",
                new Dictionary<string, object> { { "arg", null } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", null);

            result = await ExecuteQuery(
                @"{ enumField: nullableEnumField(enumArg: FOO) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", "FOO");

            result = await ExecuteQuery(
                @"query Test($arg: TestEnum) { enumField: nullableEnumField(enumArg: $arg) }",
                new Dictionary<string, object> { { "arg", "FOO" } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", "FOO");

            result = await ExecuteQuery(
                @"{ enumField: nullableEnumField(enumArg: BAZ) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", "BAZ");

            result = await ExecuteQuery(
                @"query Test($arg: TestEnum) { enumField: nullableEnumField(enumArg: $arg) }",
                new Dictionary<string, object> { { "arg", "BAZ" } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", "BAZ");
        }

        [Test]
        public async Task Can_Resolve_NonNullable_Enum_Argument()
        {
            var result = await ExecuteQuery(
                @"{ enumField: nonNullableEnumField(enumArg: FOO) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", "FOO");

            result = await ExecuteQuery(
                @"query Test($arg: TestEnum!) { enumField: nonNullableEnumField(enumArg: $arg) }",
                new Dictionary<string, object> { { "arg", "FOO" } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", "FOO");

            result = await ExecuteQuery(
                @"{ enumField: nonNullableEnumField(enumArg: BAZ) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", "BAZ");

            result = await ExecuteQuery(
                @"query Test($arg: TestEnum!) { enumField: nonNullableEnumField(enumArg: $arg) }",
                new Dictionary<string, object> { { "arg", "BAZ" } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", "BAZ");
        }

        [Test]
        public async Task Can_Resolve_List_Of_Nullable_Enums_Argument()
        {
            var result = await ExecuteQuery(
                @"{ listOfNullableEnumsField(arg: [BAZ, FOO]) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("listOfNullableEnumsField", "BAZ");

            result = await ExecuteQuery(
                @"query Test($arg: [TestEnum]) { listOfNullableEnumsField(arg: $arg) }",
                new Dictionary<string, object> { { "arg", new object[] { "BAZ", null, "FOO" } } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("listOfNullableEnumsField", "BAZ");
        }

        [Test]
        public async Task Can_Resolve_List_Of_NonNullable_Enums_Argument()
        {
            var result = await ExecuteQuery(
                @"{ listOfNonNullableEnumsField(arg: [BAZ, FOO]) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("listOfNonNullableEnumsField", "BAZ");

            result = await ExecuteQuery(
                @"query Test($arg: [TestEnum!]) { listOfNonNullableEnumsField(arg: $arg) }",
                new Dictionary<string, object> { { "arg", new object[] { "BAZ", "FOO" } } });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("listOfNonNullableEnumsField", "BAZ");
        }

        [Test]
        public async Task Can_Resolve_Nullable_InputObject_Argument()
        {
            var result = await ExecuteQuery(
                @"{ result: nullableInputObjectField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", null);

            result = await ExecuteQuery(
                @"{ result: nullableInputObjectField(arg: { field1: ""A"", field2: ""B"", field3: 1, field4: 2 }) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", "A-B-1-2");

            result = await ExecuteQuery(
                @"query Test($arg: InputObject) { result: nullableInputObjectField(arg: $arg) }",
                new Dictionary<string, object>
                {
                    {
                        "arg", new Dictionary<string, object>
                        {
                            { "field1", "A" },
                            { "field2", "B" },
                            { "field3", 1 },
                            { "field4", 2 },
                        }
                    }
                });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", "A-B-1-2");
        }

        [Test]
        public async Task Can_Resolve_NonNullable_InputObject_Argument()
        {
            var result = await ExecuteQuery(
                @"{ result: nonNullableInputObjectField }");
            result.ShouldHaveErrors(1);

            result = await ExecuteQuery(
                @"{ result: nonNullableInputObjectField(arg: { field1: ""A"", field2: ""B"", field3: 1, field4: 2 }) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", "A-B-1-2");

            result = await ExecuteQuery(
                @"query Test($arg: InputObject!) { result: nonNullableInputObjectField(arg: $arg) }",
                new Dictionary<string, object>
                {
                    {
                        "arg", new Dictionary<string, object>
                        {
                            { "field1", "A" },
                            { "field2", "B" },
                            { "field3", 1 },
                            { "field4", 2 },
                        }
                    }
                });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", "A-B-1-2");
        }

        [Test]
        public async Task Can_Resolve_Array_Of_Nullable_InputObject_Argument()
        {
            var result = await ExecuteQuery(
                @"{ result: arrayOfNullableInputObjectsField(arg: [
                            { field1: ""A"", field2: ""B"", field3: 1, field4: 2 },
                            { field1: ""X"", field2: ""Y"", field3: 9, field4: 0 } ]) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, "X-Y-9-0");

            result = await ExecuteQuery(
                @"query Test($arg: [InputObject]) { result: arrayOfNullableInputObjectsField(arg: $arg) }",
                new Dictionary<string, object>
                {
                    {
                        "arg", new object[]
                        {
                            new Dictionary<string, object>
                            {
                                { "field1", "A" },
                                { "field2", "B" },
                                { "field3", 1 },
                                { "field4", 2 },
                            },
                            null,
                            new Dictionary<string, object>
                            {
                                { "field1", "X" },
                                { "field2", "Y" },
                                { "field3", 9 },
                                { "field4", 0 },
                            },
                        }
                    }
                });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, null);
            result.Data.ShouldHaveFieldWithValue("result", 2, "X-Y-9-0");
        }

        [Test]
        public async Task Can_Resolve_Array_Of_NonNullable_InputObject_Argument()
        {
            var result = await ExecuteQuery(
                @"{ result: arrayOfNonNullableInputObjectsField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", null);

            result = await ExecuteQuery(
                @"{ result: arrayOfNonNullableInputObjectsField(arg: [
                            { field1: ""A"", field2: ""B"", field3: 1, field4: 2 },
                            { field1: ""X"", field2: ""Y"", field3: 9, field4: 0 } ]) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, "X-Y-9-0");

            result = await ExecuteQuery(
                @"query Test($arg: [InputObject!]) { result: arrayOfNonNullableInputObjectsField(arg: $arg) }",
                new Dictionary<string, object>
                {
                    {
                        "arg", new object[]
                        {
                            new Dictionary<string, object>
                            {
                                { "field1", "A" },
                                { "field2", "B" },
                                { "field3", 1 },
                                { "field4", 2 },
                            },
                            new Dictionary<string, object>
                            {
                                { "field1", "X" },
                                { "field2", "Y" },
                                { "field3", 9 },
                                { "field4", 0 },
                            },
                        }
                    }
                });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, "X-Y-9-0");
        }

        [Test]
        public async Task Can_Resolve_List_Of_Nullable_InputObject_Argument()
        {
            var result = await ExecuteQuery(
                @"{ result: listOfNullableInputObjectsField(arg: [
                            { field1: ""A"", field2: ""B"", field3: 1, field4: 2 },
                            { field1: ""X"", field2: ""Y"", field3: 9, field4: 0 } ]) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, "X-Y-9-0");

            result = await ExecuteQuery(
                @"query Test($arg: [InputObject]) { result: listOfNullableInputObjectsField(arg: $arg) }",
                new Dictionary<string, object>
                {
                    {
                        "arg", new object[]
                        {
                            new Dictionary<string, object>
                            {
                                { "field1", "A" },
                                { "field2", "B" },
                                { "field3", 1 },
                                { "field4", 2 },
                            },
                            null,
                            new Dictionary<string, object>
                            {
                                { "field1", "X" },
                                { "field2", "Y" },
                                { "field3", 9 },
                                { "field4", 0 },
                            },
                        }
                    }
                });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, null);
            result.Data.ShouldHaveFieldWithValue("result", 2, "X-Y-9-0");
        }

        [Test]
        public async Task Can_Resolve_List_Of_NonNullable_InputObject_Argument()
        {
            var result = await ExecuteQuery(
                @"{ result: listOfNonNullableInputObjectsField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", null);

            result = await ExecuteQuery(
                @"{ result: listOfNonNullableInputObjectsField(arg: [
                            { field1: ""A"", field2: ""B"", field3: 1, field4: 2 },
                            { field1: ""X"", field2: ""Y"", field3: 9, field4: 0 } ]) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, "X-Y-9-0");

            result = await ExecuteQuery(
                @"query Test($arg: [InputObject!]) { result: listOfNonNullableInputObjectsField(arg: $arg) }",
                new Dictionary<string, object>
                {
                    {
                        "arg", new object[]
                        {
                            new Dictionary<string, object>
                            {
                                { "field1", "A" },
                                { "field2", "B" },
                                { "field3", 1 },
                                { "field4", 2 },
                            },
                            new Dictionary<string, object>
                            {
                                { "field1", "X" },
                                { "field2", "Y" },
                                { "field3", 9 },
                                { "field4", 0 },
                            },
                        }
                    }
                });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, "X-Y-9-0");
       }

        [Test]
        public async Task Can_Resolve_Enumerable_Of_Nullable_InputObject_Argument()
        {
            var result = await ExecuteQuery(
                @"{ result: enumerableOfNullableInputObjectsField(arg: [
                            { field1: ""A"", field2: ""B"", field3: 1, field4: 2 },
                            { field1: ""X"", field2: ""Y"", field3: 9, field4: 0 } ]) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, "X-Y-9-0");

            result = await ExecuteQuery(
                @"query Test($arg: [InputObject]) { result: enumerableOfNullableInputObjectsField(arg: $arg) }",
                new Dictionary<string, object>
                {
                    {
                        "arg", new object[]
                        {
                            new Dictionary<string, object>
                            {
                                { "field1", "A" },
                                { "field2", "B" },
                                { "field3", 1 },
                                { "field4", 2 },
                            },
                            null,
                            new Dictionary<string, object>
                            {
                                { "field1", "X" },
                                { "field2", "Y" },
                                { "field3", 9 },
                                { "field4", 0 },
                            },
                        }
                    }
                });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, null);
            result.Data.ShouldHaveFieldWithValue("result", 2, "X-Y-9-0");
        }

        [Test]
        public async Task Can_Resolve_Enumerable_Of_NonNullable_InputObject_Argument()
        {
            var result = await ExecuteQuery(
                @"{ result: enumerableOfNonNullableInputObjectsField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", null);

            result = await ExecuteQuery(
                @"{ result: enumerableOfNonNullableInputObjectsField(arg: [
                            { field1: ""A"", field2: ""B"", field3: 1, field4: 2 },
                            { field1: ""X"", field2: ""Y"", field3: 9, field4: 0 } ]) }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, "X-Y-9-0");

            result = await ExecuteQuery(
                @"query Test($arg: [InputObject!]) { result: enumerableOfNonNullableInputObjectsField(arg: $arg) }",
                new Dictionary<string, object>
                {
                    {
                        "arg", new object[]
                        {
                            new Dictionary<string, object>
                            {
                                { "field1", "A" },
                                { "field2", "B" },
                                { "field3", 1 },
                                { "field4", 2 },
                            },
                            new Dictionary<string, object>
                            {
                                { "field1", "X" },
                                { "field2", "Y" },
                                { "field3", 9 },
                                { "field4", 0 },
                            },
                        }
                    }
                });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", 0, "A-B-1-2");
            result.Data.ShouldHaveFieldWithValue("result", 1, "X-Y-9-0");
        }

        [Test]
        public async Task Can_Resolve_Complex_Input_Object()
        {
            var result = await ExecuteQuery(
                @"{ result: complexInputObjectField }");
            result.ShouldHaveErrors(1);

            result = await ExecuteQuery(
                @"{ result: complexInputObjectField(arg:
                        {
                            identifier: """ + Id.New<InputObject>("12345") + @""",
                            nullableObjects: [
                                { field1: ""Foo"", field2: ""Bar"", field3: 10, field4: 99 },
                                { field1: ""Baz"", field2: ""Foo"", field3: 33, field4: 44 }
                            ],
                            nonNullableObjects: [
                                { field1: ""Foo"", field2: ""Bar"", field3: 10, field4: 99 },
                                { field1: ""Baz"", field2: ""Foo"", field3: 33, field4: 44 }
                            ],
                            listOfEnums: [ BAZ, FOO ]
                        })
                  }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", "SW5wdXRPYmplY3Q6MTIzNDU=-Foo-33-Bar");

            var obj1 = new Dictionary<string, object>
            {
                { "field1", "Foo" },
                { "field2", "Bar" },
                { "field3", 10 },
                { "field4", 99 },
            };
            var obj2 = new Dictionary<string, object>
            {
                { "field1", "Baz" },
                { "field2", "Foo" },
                { "field3", 33},
                { "field4", 44 },
            };

            result = await ExecuteQuery(
                @"query Test($arg: ComplexInputObject!) { result: complexInputObjectField(arg: $arg) }",
                new Dictionary<string, object>
                {
                    {
                        "arg", new Dictionary<string, object>
                        {
                            { "identifier", Id.New<InputObject>("12345").ToString() },
                            { "nullableObjects", new object[] { obj1, obj2 } },
                            { "nonNullableObjects", new object[] { obj1, obj2 } },
                            { "listOfEnums", new object[] { "BAZ", "FOO" } },
                        }
                    }
                });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("result", "SW5wdXRPYmplY3Q6MTIzNDU=-Foo-33-Bar");
        }

        [Test]
        public async Task Can_Resolve_Resolution_Context_Argument()
        {
            var result = await ExecuteQuery(
                @"{ contextField }",
                userContext: new UserContext { SomeValue = 15 + 10 });
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("contextField", "25");
        }

        private async Task<ExecutionResult> ExecuteQuery(
            string query, Dictionary<string, object> inputs = null, IUserContext userContext = null)
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString(query)
                .WithInputs(inputs)
                .WithUserContext(userContext)
                .WithDependencyInjector(new DependencyInjector())
                .Execute();
            return result;
        }

        class UserContext : IUserContext
        {
            public int SomeValue { get; set; }
        }

        class DependencyInjector : IDependencyInjector
        {
            public object Resolve(TypeInfo typeInfo)
            {
                if (typeInfo.AsType() == typeof(IDependency))
                {
                    return new Dependency();
                }
                return null;
            }
        }

        interface IDependency
        {
            string GetValue();
        }

        class Dependency : IDependency
        {
            public string GetValue() => "Injection";
        }

        class Query
        {
            public string DependencyInjectionField([Inject] IDependency dependency) =>
                dependency.GetValue();

            public int ValueTypeField(int valueTypeArg) =>
                valueTypeArg;

            public int? NullableValueTypeField(int? nullableValueTypeArg) =>
                nullableValueTypeArg;

            public string NullablePrimitiveField(string nullablePrimitiveArg) =>
                nullablePrimitiveArg;

            public NonNull<string> NonNullablePrimitiveField(NonNull<string> nonNullablePrimitiveArg) =>
                nonNullablePrimitiveArg;

            public int? DefaultNullableArgumentValueField(int? arg = 999) =>
                arg;

            public int? DefaultNonNullableArgumentValueField(int arg = 999) =>
                arg;

            public string NonNullableIdField(Id idArg) =>
                idArg.IdentifierForType<Dependency>();

            public string NullableIdField(Id? idArg) =>
                idArg?.IdentifierForType<Dependency>();

            public TestEnum NonNullableEnumField(TestEnum enumArg) =>
                enumArg;

            public TestEnum? NullableEnumField(TestEnum? enumArg) =>
                enumArg;

            public TestEnum ListOfNonNullableEnumsField(List<TestEnum> arg) =>
                arg.First();

            public TestEnum? ListOfNullableEnumsField(List<TestEnum?> arg) =>
                arg.FirstOrDefault();

            public string NullableInputObjectField(InputObject arg) =>
                arg?.ToString();

            public string NonNullableInputObjectField(NonNull<InputObject> arg) =>
                arg.Value.ToString();

            public IEnumerable<string> ArrayOfNullableInputObjectsField(InputObject[] arg) =>
                arg?.Select(obj => obj?.ToString());

            public IEnumerable<string> ArrayOfNonNullableInputObjectsField(NonNull<InputObject>[] arg) =>
                arg?.Select(obj => obj.Value.ToString());

            public IEnumerable<string> ListOfNullableInputObjectsField(List<InputObject> arg) =>
                arg?.Select(obj => obj?.ToString());

            public IEnumerable<string> ListOfNonNullableInputObjectsField(List<NonNull<InputObject>> arg) =>
                arg?.Select(obj => obj.Value.ToString());

            public IEnumerable<string> EnumerableOfNullableInputObjectsField(IEnumerable<InputObject> arg) =>
                arg?.Select(obj => obj?.ToString());

            public IEnumerable<string> EnumerableOfNonNullableInputObjectsField(IEnumerable<NonNull<InputObject>> arg) =>
                arg?.Select(obj => obj.Value.ToString());

            public string ComplexInputObjectField(NonNull<ComplexInputObject> arg) =>
                $"{arg.Value.Identifier}-{arg.Value.NullableObjects.First().Field1}-" +
                $"{arg.Value.NonNullableObjects.Last().Value.Field3}-{arg.Value.ListOfEnums.Value.First()}";

            public string ContextField(UserContext context) =>
                context.SomeValue.ToString();
        }

        enum TestEnum
        {
            Foo,

            [Name("BAZ")]
            Bar,
        }

        [InputType]
        class InputObject
        {
            public string Field1 { get; set; }

            public NonNull<string> Field2 { get; set; }

            public int Field3 { get; set; }

            public int? Field4 { get; set; }

            public override string ToString() =>
                $"{Field1}-{Field2}-{Field3}-{Field4}";
        }

        [InputType]
        class ComplexInputObject
        {
            public Id Identifier { get; set; }

            public List<InputObject> NullableObjects { get; set; }

            public List<NonNull<InputObject>> NonNullableObjects { get; set; }

            public NonNull<List<TestEnum>> ListOfEnums { get; set; }
        }
    }
}