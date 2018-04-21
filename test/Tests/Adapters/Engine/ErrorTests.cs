using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Adapters.Engine
{
    public class ErrorTests : TestBase
    {
        [Test]
        public async void Will_Provide_Path_And_Code_For_Errors_On_Fields_With_Operation_Name()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("query Blah { getObject { field { test } } }")
                .Execute();

            result.Errors.ShouldNotBeNull();
            result.Errors.Count().ShouldEqual(1);
            var error = result.Errors.First();
            error.Message.ShouldEqual("Test error.");
            error.Code.ShouldEqual("ARGUMENT");
            error.Path.Count().ShouldEqual(3);
            error.Path.ElementAt(0).ShouldEqual("getObject");
            error.Path.ElementAt(1).ShouldEqual("field");
            error.Path.ElementAt(2).ShouldEqual("test");
        }

        [Test]
        public async void Will_Provide_Path_And_Code_For_Errors_On_Fields_Without_Operation_Name()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ getObject { field { test } } }")
                .Execute();

            result.Errors.ShouldNotBeNull();
            result.Errors.Count().ShouldEqual(1);
            var error = result.Errors.First();
            error.Message.ShouldEqual("Test error.");
            error.Code.ShouldEqual("ARGUMENT");
            error.Path.Count().ShouldEqual(3);
            error.Path.ElementAt(0).ShouldEqual("getObject");
            error.Path.ElementAt(1).ShouldEqual("field");
            error.Path.ElementAt(2).ShouldEqual("test");
        }

        [Test]
        public async void Will_Provide_Path_And_Code_For_Errors_On_Fields_With_Aliases()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ yo: getObject { foo: field { bar: test } } }")
                .Execute();

            result.Errors.ShouldNotBeNull();
            result.Errors.Count().ShouldEqual(1);
            var error = result.Errors.First();
            error.Message.ShouldEqual("Test error.");
            error.Code.ShouldEqual("ARGUMENT");
            error.Path.Count().ShouldEqual(3);
            error.Path.ElementAt(0).ShouldEqual("getObject");
            error.Path.ElementAt(1).ShouldEqual("field");
            error.Path.ElementAt(2).ShouldEqual("test");
        }

        [Test]
        public async void Will_Provide_Path_And_Code_For_Errors_In_Array_Fields()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("query Blah { getObject { arrayField { test } } }")
                .Execute();

            result.Data.ShouldHaveFieldWithValue("getObject", "arrayField", 0, "test", "some value");
            result.Data.ShouldHaveFieldWithValue("getObject", "arrayField", 1, "test", null);
            result.Data.ShouldHaveFieldWithValue("getObject", "arrayField", 2, "test", "some value");
            result.Data.ShouldHaveFieldWithValue("getObject", "arrayField", 3, "test", null);

            result.Errors.ShouldNotBeNull();
            result.Errors.Count().ShouldEqual(2);

            var error = result.Errors.ElementAt(0);
            error.Message.ShouldEqual("Test error.");
            error.Code.ShouldEqual("ARGUMENT");
            error.Path.Count().ShouldEqual(4);
            error.Path.ElementAt(0).ShouldEqual("getObject");
            error.Path.ElementAt(1).ShouldEqual("arrayField");
            error.Path.ElementAt(2).ShouldEqual("1");
            error.Path.ElementAt(3).ShouldEqual("test");

            error = result.Errors.ElementAt(1);
            error.Message.ShouldEqual("Test error.");
            error.Code.ShouldEqual("ARGUMENT");
            error.Path.Count().ShouldEqual(4);
            error.Path.ElementAt(0).ShouldEqual("getObject");
            error.Path.ElementAt(1).ShouldEqual("arrayField");
            error.Path.ElementAt(2).ShouldEqual("3");
            error.Path.ElementAt(3).ShouldEqual("test");
        }

        [Test]
        public async void Will_Provide_Exception_Data()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("query Blah { errorWithData }")
                .Execute();

            result.Data.ShouldHaveFieldWithValue("errorWithData", null);

            result.Errors.ShouldNotBeNull();
            result.Errors.Count().ShouldEqual(1);

            result.Errors.First().Message.ShouldEqual("Test error.");
            result.Errors.First().Data["someKey"].ShouldEqual("someValue");
        }

        class Query
        {
            public Object1 GetObject() => new Object1();

            public string ErrorWithData()
            {
                var exception = new Exception("Test error.");
                exception.Data["someKey"] = "someValue";
                throw exception;
            }
        }

        class Object1
        {
            public Object2 Field => new Object2();

            public List<Object2> ArrayField => new List<Object2>
            {
                new Object2(false),
                new Object2(true),
                new Object2(false),
                new Object2(true),
            };
        }

        class Object2
        {
            private readonly bool _throwError;

            public Object2(bool throwError = true)
            {
                _throwError = throwError;
            }

            public string Test()
            {
                if (_throwError)
                {
                    throw new ArgumentException("Test error.");
                }
                return "some value";
            }
        }
    }
}
