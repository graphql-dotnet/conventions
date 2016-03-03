using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Conventions.Adapters.Engine;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Relay;
using Xunit;

namespace GraphQL.Conventions.Tests.Adapters
{
    public class FieldResolutionTests : TestBase
    {
        [Fact]
        public async void Can_Resolve_NonNullable_Field()
        {
            var result = await ExecuteQuery("{ nonNullBooleanField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullBooleanField", true);
        }

        public async void Can_Resolve_Nullable_Field()
        {
            var result = await ExecuteQuery("{ booleanField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("booleanField", true);
        }

        [Fact]
        public async void Can_Resolve_NonNullable_Task_Field()
        {
            var result = await ExecuteQuery("{ nonNullBooleanTaskField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullBooleanTaskField", true);
        }

        [Fact]
        public async void Can_Resolve_Nullable_Task_Field()
        {
            var result = await ExecuteQuery("{ booleanTaskField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("booleanTaskField", true);
        }

        [Fact]
        public async void Can_Resolve_NonNullable_Enum_Field()
        {
            var result = await ExecuteQuery("{ nonNullEnumField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullEnumField", "TWO");
        }

        public async void Can_Resolve_Nullable_Enum_Field()
        {
            var result = await ExecuteQuery("{ enumField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumField", "ONE");
        }

        [Fact]
        public async void Can_Resolve_NonNullable_Enum_Task_Field()
        {
            var result = await ExecuteQuery("{ nonNullEnumTaskField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullEnumTaskField", "FOUR");
        }

        [Fact]
        public async void Can_Resolve_Nullable_Enum_Task_Field()
        {
            var result = await ExecuteQuery("{ enumTaskField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("enumTaskField", "THREE");
        }

        [Fact]
        public async void Can_Resolve_NonNullable_Id_Field()
        {
            var result = await ExecuteQuery("{ nonNullIdField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullIdField", Id.New<SimpleObject>("54321").ToString());
        }

        public async void Can_Resolve_Nullable_Id_Field()
        {
            var result = await ExecuteQuery("{ idField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("idField", Id.New<SimpleObject>("12345").ToString());
        }

        [Fact]
        public async void Can_Resolve_NonNullable_Id_Task_Field()
        {
            var result = await ExecuteQuery("{ nonNullIdTaskField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullIdTaskField", Id.New<SimpleObject>("54321").ToString());
        }

        [Fact]
        public async void Can_Resolve_Nullable_Id_Task_Field()
        {
            var result = await ExecuteQuery("{ idTaskField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("idTaskField", Id.New<SimpleObject>("12345").ToString());
        }

        [Fact]
        public async void Can_Resolve_Url_Field()
        {
            var result = await ExecuteQuery("{ urlField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("urlField", "http://www.google.com/");
        }

        [Fact]
        public async void Can_Resolve_List_Of_Nullable_Primitives_Field()
        {
            var result = await ExecuteQuery("{ list: nullablePrimitiveListField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveArrayFieldOfCount("list", 3);
            result.Data.ShouldHaveFieldWithValue("list", 0, 10);
            result.Data.ShouldHaveFieldWithValue("list", 1, null);
            result.Data.ShouldHaveFieldWithValue("list", 2, 90);
        }

        [Fact]
        public async void Can_Resolve_List_Of_Primitives_Field()
        {
            var result = await ExecuteQuery("{ list: primitiveListField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveArrayFieldOfCount("list", 3);
            result.Data.ShouldHaveFieldWithValue("list", 0, 1);
            result.Data.ShouldHaveFieldWithValue("list", 1, 2);
            result.Data.ShouldHaveFieldWithValue("list", 2, 3);
        }

        [Fact]
        public async void Can_Resolve_List_Of_Nullables_Field()
        {
            var result = await ExecuteQuery("{ list: listField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveArrayFieldOfCount("list", 3);
            result.Data.ShouldHaveFieldWithValue("list", 0, "a");
            result.Data.ShouldHaveFieldWithValue("list", 1, "b");
            result.Data.ShouldHaveFieldWithValue("list", 2, "c");
        }

        [Fact]
        public async void Can_Resolve_List_Of_NonNullables_Field()
        {
            var result = await ExecuteQuery("{ list: listOfNonNullField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveArrayFieldOfCount("list", 3);
            result.Data.ShouldHaveFieldWithValue("list", 0, "a");
            result.Data.ShouldHaveFieldWithValue("list", 1, "b");
            result.Data.ShouldHaveFieldWithValue("list", 2, "c");
        }

        [Fact]
        public async void Can_Resolve_NonNullable_List_Of_Nullables_Field()
        {
            var result = await ExecuteQuery("{ list: nonNullListField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveArrayFieldOfCount("list", 3);
            result.Data.ShouldHaveFieldWithValue("list", 0, "a");
            result.Data.ShouldHaveFieldWithValue("list", 1, "b");
            result.Data.ShouldHaveFieldWithValue("list", 2, "c");
        }

        [Fact]
        public async void Can_Resolve_NonNullable_List_Of_NonNullables_Field()
        {
            var result = await ExecuteQuery("{ list: nonNullListOfNonNullField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveArrayFieldOfCount("list", 3);
            result.Data.ShouldHaveFieldWithValue("list", 0, "a");
            result.Data.ShouldHaveFieldWithValue("list", 1, "b");
            result.Data.ShouldHaveFieldWithValue("list", 2, "c");
        }

        [Fact]
        public async void Can_Resolve_NonNullable_List_Of_NonNullable_URLs_Field()
        {
            var result = await ExecuteQuery("{ urls: nonNullListOfNonNullUrlsField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveArrayFieldOfCount("urls", 2);
            result.Data.ShouldHaveFieldWithValue("urls", 0, "http://www.amazon.com/");
            result.Data.ShouldHaveFieldWithValue("urls", 1, "http://www.google.com/");
        }

        [Fact]
        public async void Can_Resolve_NonNullable_List_Of_NonNullable_URLs_Task_Field()
        {
            var result = await ExecuteQuery("{ urls: nonNullListOfNonNullUrlsTaskField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveArrayFieldOfCount("urls", 2);
            result.Data.ShouldHaveFieldWithValue("urls", 0, "http://www.yahoo.com/");
            result.Data.ShouldHaveFieldWithValue("urls", 1, "http://www.facebook.com/");
        }

        [Fact]
        public async void Can_Resolve_Enumerable_Field()
        {
            var result = await ExecuteQuery("{ enumerableField iListField arrayField }");
            result.ShouldHaveNoErrors();

            result.Data.ShouldHaveArrayFieldOfCount("enumerableField", 5);
            result.Data.ShouldHaveFieldWithValue("enumerableField", 0, 1);
            result.Data.ShouldHaveFieldWithValue("enumerableField", 1, 2);
            result.Data.ShouldHaveFieldWithValue("enumerableField", 2, 3);
            result.Data.ShouldHaveFieldWithValue("enumerableField", 3, 4);
            result.Data.ShouldHaveFieldWithValue("enumerableField", 4, 5);

            result.Data.ShouldHaveArrayFieldOfCount("iListField", 2);
            result.Data.ShouldHaveFieldWithValue("iListField", 0, true);
            result.Data.ShouldHaveFieldWithValue("iListField", 1, false);

            result.Data.ShouldHaveArrayFieldOfCount("arrayField", 2);
            result.Data.ShouldHaveFieldWithValue("arrayField", 0, true);
            result.Data.ShouldHaveFieldWithValue("arrayField", 1, false);
        }

        [Fact]
        public async void Can_Resolve_Enumerable_Of_NonNullable_URLs_Field()
        {
            var result = await ExecuteQuery("{ urls: enumerableOfNonNullUrlsField }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveArrayFieldOfCount("urls", 2);
            result.Data.ShouldHaveFieldWithValue("urls", 0, "http://www.amazon.com/");
            result.Data.ShouldHaveFieldWithValue("urls", 1, "http://www.facebook.com/");
        }

        [Fact]
        public async void Throws_Exception_With_Correct_StackTrace_On_Error()
        {
            var result = await ExecuteQuery("{ errorField }");
            result.ShouldHaveErrors(1);
            var error = result.Errors.First();
            error.Message.ShouldContain("Error trying to resolve errorField");
            error.InnerException.Message.ShouldContain($"Unable to resolve field 'errorField' on type '{nameof(Query)}'");
            error.InnerException.ToString().ShouldContain("System.NotImplementedException: The method or operation is not implemented");
            error.InnerException.ToString().ShouldContain("at GraphQL.Conventions.Tests.Adapters.FieldResolutionTests.Query.ErrorField() in");
            error.InnerException.ToString().ShouldContain("Adapters/FieldResolutionTests.cs:line");
        }

        [Fact]
        public async void Throws_Exception_With_Correct_StackTrace_On_Task_Error()
        {
            var result = await ExecuteQuery("{ errorTaskField }");
            result.ShouldHaveErrors(1);
            var error = result.Errors.First();
            error.Message.ShouldContain("Error trying to resolve errorTaskField");
            error.InnerException.Message.ShouldContain($"Unable to resolve field 'errorTaskField' on type '{nameof(Query)}'");
            error.InnerException.ToString().ShouldContain("System.NotImplementedException: The method or operation is not implemented");
            error.InnerException.ToString().ShouldContain("at GraphQL.Conventions.Tests.Adapters.FieldResolutionTests.Query.ErrorTaskField() in");
            error.InnerException.ToString().ShouldContain("Adapters/FieldResolutionTests.cs:line");
        }

        [Fact]
        public async void Can_Resolve_NonNullable_Object_Field()
        {
            var result = await ExecuteQuery(@"
            {
                obj: nonNullObjectField { foo bar }
            }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("obj", "foo", "Hello");
            result.Data.ShouldHaveFieldWithValue("obj", "bar", 3.14);
        }

        [Fact]
        public async void Can_Resolve_Nullable_Object_Field()
        {
            var result = await ExecuteQuery(@"
            {
                obj: objectField { foo bar }
            }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("obj", "foo", "Hello");
            result.Data.ShouldHaveFieldWithValue("obj", "bar", 3.14);
        }

        [Fact]
        public async void Can_Resolve_NonNullable_Object_Task_Field()
        {
            var result = await ExecuteQuery(@"
            {
                obj: nonNullObjectTaskField { foo bar }
            }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("obj", "foo", "Task");
            result.Data.ShouldHaveFieldWithValue("obj", "bar", 1.0);
        }

        [Fact]
        public async void Can_Resolve_Nullable_Object_Task_Field()
        {
            var result = await ExecuteQuery(@"
            {
                obj: objectTaskField { foo bar }
            }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("obj", "foo", "Task");
            result.Data.ShouldHaveFieldWithValue("obj", "bar", 1.0);
        }

        [Fact]
        public async void Can_Resolve_Interface_Field()
        {
            var result = await ExecuteQuery(@"
            {
                interfaceField
                {
                    value
                    ... on InterfaceImplementation1 { __typename test: test1 }
                    ... on InterfaceImplementation2 { __typename test: test2 }
                }
                nonNullInterfaceField
                {
                    value
                    ... on InterfaceImplementation1 { __typename test: test1 }
                    ... on InterfaceImplementation2 { __typename test: test2 }
                }
                nonNullInterfaceTaskField
                {
                    value
                    ... on InterfaceImplementation1 { __typename test: test1 }
                    ... on InterfaceImplementation2 { __typename test: test2 }
                }
            }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("interfaceField", "__typename", "InterfaceImplementation1");
            result.Data.ShouldHaveFieldWithValue("interfaceField", "value", "Test 1");
            result.Data.ShouldHaveFieldWithValue("interfaceField", "test", 1);
            result.Data.ShouldHaveFieldWithValue("nonNullInterfaceField", "__typename", "InterfaceImplementation2");
            result.Data.ShouldHaveFieldWithValue("nonNullInterfaceField", "value", "Test 2");
            result.Data.ShouldHaveFieldWithValue("nonNullInterfaceField", "test", 2);
            result.Data.ShouldHaveFieldWithValue("nonNullInterfaceTaskField", "__typename", "InterfaceImplementation2");
            result.Data.ShouldHaveFieldWithValue("nonNullInterfaceTaskField", "value", "Test 2");
            result.Data.ShouldHaveFieldWithValue("nonNullInterfaceTaskField", "test", 2);
        }

        [Fact]
        public async void Can_Resolve_Union_Field()
        {
            var result = await ExecuteQuery(@"
            {
                unionField
                {
                    ... on UnionImplementation1 { __typename field: field1 }
                    ... on UnionImplementation2 { __typename field: field2 }
                }
                nonNullUnionField
                {
                    ... on UnionImplementation1 { __typename field: field1 }
                    ... on UnionImplementation2 { __typename field: field2 }
                }
                nonNullUnionTaskField
                {
                    ... on UnionImplementation1 { __typename field: field1 }
                    ... on UnionImplementation2 { __typename field: field2 }
                }
            }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("unionField", "__typename", "UnionImplementation1");
            result.Data.ShouldHaveFieldWithValue("unionField", "field", "First");
            result.Data.ShouldHaveFieldWithValue("nonNullUnionField", "__typename", "UnionImplementation2");
            result.Data.ShouldHaveFieldWithValue("nonNullUnionField", "field", "Second");
            result.Data.ShouldHaveFieldWithValue("nonNullUnionTaskField", "__typename", "UnionImplementation2");
            result.Data.ShouldHaveFieldWithValue("nonNullUnionTaskField", "field", "Second");
        }

        [Fact]
        public async void Can_Resolve_Asynchronous_Field()
        {
            var result = await ExecuteQuery(@"
            {
                asyncField { data }
                nonNullAsyncField { data }
                asyncListField { data }
            }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("asyncField", "data", "Some Value");
            result.Data.ShouldHaveFieldWithValue("nonNullAsyncField", "data", "Some Other Value");
            result.Data.ShouldHaveArrayFieldOfCount("asyncListField", 3);
            result.Data.ShouldHaveFieldWithValue("asyncListField", 0, "data", "Entity 1");
            result.Data.ShouldHaveFieldWithValue("asyncListField", 1, "data", "Entity 2");
            result.Data.ShouldHaveFieldWithValue("asyncListField", 2, "data", "Entity 3");
        }

        private async Task<ExecutionResult> ExecuteQuery(string query)
        {
            var engine = new GraphQLEngine(typeof(SchemaDefinition<Query>));
            var result = await engine
                .NewExecutor()
                .WithQueryString(query)
                .Execute();
            return result;
        }

        class Query
        {
            public bool? BooleanField => true;

            public bool NonNullBooleanField => true;

            public Task<bool?> BooleanTaskField() => Task.FromResult((bool?)true);

            public Task<bool> NonNullBooleanTaskField() => Task.FromResult(true);

            public TestEnum? EnumField => TestEnum.One;

            public TestEnum NonNullEnumField => TestEnum.Two;

            public Task<TestEnum?> EnumTaskField() => Task.FromResult((TestEnum?)TestEnum.Three);

            public Task<TestEnum> NonNullEnumTaskField() => Task.FromResult(TestEnum.Four);

            public Id? IdField => Id.New<SimpleObject>("12345");

            public Id NonNullIdField => Id.New<SimpleObject>("54321");

            public Task<Id?> IdTaskField() => Task.FromResult((Id?)Id.New<SimpleObject>("12345"));

            public Task<Id> NonNullIdTaskField() => Task.FromResult(Id.New<SimpleObject>("54321"));

            public Url UrlField => new Url("http://www.google.com/");

            public SimpleObject ObjectField =>
                new SimpleObject
                {
                    Foo = "Hello",
                    Bar = 3.14,
                };

            public NonNull<SimpleObject> NonNullObjectField =>
                new SimpleObject
                {
                    Foo = "Hello",
                    Bar = 3.14,
                };

            public Task<SimpleObject> ObjectTaskField() => Task.FromResult(
                new SimpleObject
                {
                    Foo = "Task",
                    Bar = 1.0,
                });

            public Task<NonNull<SimpleObject>> NonNullObjectTaskField() => Task.FromResult(
                new NonNull<SimpleObject>(new SimpleObject
                {
                    Foo = "Task",
                    Bar = 1.0,
                }));

            public bool ErrorField()
            {
                throw new System.NotImplementedException();
            }

            public Task<bool> ErrorTaskField()
            {
                throw new System.NotImplementedException();
            }

            public TestInterface InterfaceField => new InterfaceImplementation1();

            public NonNull<TestInterface> NonNullInterfaceField() => new InterfaceImplementation2();

            public Task<NonNull<TestInterface>> NonNullInterfaceTaskField() => Task.FromResult(new NonNull<TestInterface>(new InterfaceImplementation2()));

            public TestUnion UnionField => new TestUnion { Instance = new UnionImplementation1() };

            public NonNull<TestUnion> NonNullUnionField() => new TestUnion { Instance = new UnionImplementation2() };

            public Task<NonNull<TestUnion>> NonNullUnionTaskField() => Task.FromResult(new NonNull<TestUnion>(new TestUnion { Instance = new UnionImplementation2() }));

            public List<int?> NullablePrimitiveListField() => new List<int?> { 10, null, 90 };

            public List<int> PrimitiveListField() => new List<int> { 1, 2, 3 };

            public List<string> ListField() => new List<string> { "a", "b", "c" };

            public NonNull<List<string>> NonNullListField() => new List<string> { "a", "b", "c" };

            public List<NonNull<string>> ListOfNonNullField() => new List<NonNull<string>>
            {
                new NonNull<string>("a"),
                new NonNull<string>("b"),
                new NonNull<string>("c"),
            };

            public NonNull<List<NonNull<string>>> NonNullListOfNonNullField() => new List<NonNull<string>>
            {
                new NonNull<string>("a"),
                new NonNull<string>("b"),
                new NonNull<string>("c"),
            };

            public NonNull<List<NonNull<Url>>> NonNullListOfNonNullUrlsField() => new List<NonNull<Url>>
            {
                new NonNull<Url>(new Url("http://www.amazon.com/")),
                new NonNull<Url>(new Url("http://www.google.com/")),
            };

            public async Task<NonNull<IEnumerable<NonNull<Url>>>> NonNullListOfNonNullUrlsTaskField()
            {
                await Task.Delay(1);
                return new List<NonNull<Url>>
                {
                    new NonNull<Url>(new Url("http://www.yahoo.com/")),
                    new NonNull<Url>(new Url("http://www.facebook.com/")),
                };
            }

            public IEnumerable<int> EnumerableField()
            {
                for (var i = 1; i <= 5; i++)
                {
                    yield return i;
                }
            }

            public IEnumerable<NonNull<Url>> EnumerableOfNonNullUrlsField()
            {
                yield return new Url("http://www.amazon.com/");
                yield return new Url("http://www.facebook.com/");
            }

            public IList<bool> IListField => new List<bool> { true, false };

            public bool[] ArrayField => new[] { true, false };

            public async Task<Entity> AsyncField()
            {
                await Task.Delay(1);
                return new Entity("Some Value");
            }

            public async Task<NonNull<Entity>> NonNullAsyncField()
            {
                await Task.Delay(1);
                return new Entity("Some Other Value");
            }

            public async Task<List<Entity>> AsyncListField()
            {
                await Task.Delay(1);
                return new List<Entity>
                {
                    new Entity("Entity 1"),
                    new Entity("Entity 2"),
                    new Entity("Entity 3"),
                };
            }

            public Connection<SimpleObject> TestConnection() => null;
        }

        class SimpleObject
        {
            public string Foo { get; set; }

            public double Bar { get; set; }
        }

        enum TestEnum
        {
            One,
            Two,
            Three,
            Four,
        }

        class Entity
        {
            private readonly string _data;

            public Entity(string data)
            {
                _data = data;
            }

            public async Task<NonNull<string>> Data()
            {
                await Task.Delay(10);
                return _data;
            }
        }

        public interface TestInterface
        {
            string Value { get; }
        }

        class InterfaceImplementation1 : TestInterface
        {
            public string Value => "Test 1";

            public int Test1 => 1;
        }

        class InterfaceImplementation2 : TestInterface
        {
            public string Value => "Test 2";

            public int Test2 => 2;
        }

        class UnionImplementation1
        {
            public string Field1 => "First";
        }

        class UnionImplementation2
        {
            public string Field2 => "Second";
        }

        class TestUnion : Union<UnionImplementation1, UnionImplementation2>
        {
        }
    }
}