using System;
using System.Collections.Generic;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using Newtonsoft.Json;

namespace GraphQL.Conventions.Tests.Adapters.Engine.Utilities
{
    public class RequestDeserializerTests : TestBase
    {
        private readonly IRequestDeserializer _deserializer = new RequestDeserializer();

        [Test]
        public void Throws_On_Invalid_Input()
        {
            ShouldThrow<ArgumentException>(() => Query(InvalidRequest).QueryString.ShouldEqual(string.Empty));
            ShouldThrow<JsonReaderException>(() => Query(MalformedRequest).QueryString.ShouldEqual(string.Empty));
        }

        [Test]
        public void Can_Derive_Query_String_From_Request()
        {
            Query(EmptyRequest).QueryString.ShouldEqual(string.Empty);
            Query(RequestWithQuery).QueryString.ShouldEqual(QueryString);
            Query(RequestWithOperation).QueryString.ShouldEqual(QueryString);
            Query(RequestWithUnserializedVariables).QueryString.ShouldEqual(QueryString);
            Query(RequestWithSerializedVariables).QueryString.ShouldEqual(QueryString);
        }

        [Test]
        public void Can_Derive_Operation_Name_From_Request()
        {
            Query(EmptyRequest).OperationName.ShouldEqual(null);
            Query(RequestWithQuery).OperationName.ShouldEqual(null);
            Query(RequestWithOperation).OperationName.ShouldEqual("T");
            Query(RequestWithUnserializedVariables).OperationName.ShouldEqual("T");
            Query(RequestWithSerializedVariables).OperationName.ShouldEqual("T");
        }

        [Test]
        public void Can_Derive_Inputs_From_Stringified_Field()
        {
            Query(EmptyRequest).Variables.ShouldEqual(null);
            Query(RequestWithQuery).Variables.ShouldEqual(null);
            Query(RequestWithOperation).Variables.ShouldEqual(null);

            var variables = Query(RequestWithSerializedVariables).Variables;
            variables.ShouldNotBeNull();
            variables.Count.ShouldEqual(1);
            variables.Keys.ShouldContain("bool");
            variables["bool"].ShouldEqual(true);
        }

        [Test]
        public void Can_Derive_Inputs_From_Unserialized_Field()
        {
            Query(EmptyRequest).Variables.ShouldEqual(null);
            Query(RequestWithQuery).Variables.ShouldEqual(null);
            Query(RequestWithOperation).Variables.ShouldEqual(null);

            var variables = Query(RequestWithUnserializedVariables).Variables;
            variables.ShouldNotBeNull();
            variables.Count.ShouldEqual(1);
            variables.Keys.ShouldContain("bool");
            variables["bool"].ShouldEqual(true);
        }

        [Test]
        public void Can_Derive_Complex_Inputs_From_Field()
        {
            var variables = Query(RequestWithComplexVariables).Variables;
            variables.ShouldNotBeNull();
            variables.Count.ShouldEqual(1);
            variables.Keys.ShouldContain("obj");

            var obj = variables["obj"] as Dictionary<string, object>;
            obj.ShouldNotBeNull();
            obj.Count.ShouldEqual(2);
            obj.Keys.ShouldContain("field1");
            obj.Keys.ShouldContain("field2");
            obj["field1"].ShouldEqual(1);
            obj["field2"].ShouldEqual(null);
        }

        private QueryInput Query(string requestBody) => _deserializer.GetQueryFromRequestBody(requestBody);

        private const string QueryString = "query T($bool: Boolean!) {\n  version @include(if: $bool)\n}";

        private const string SerializedVariables = "\"{\n  \\\"bool\\\": true\n}\"";

        private const string UnserializedVariables = "{ \"bool\": true }";

        private const string VariablesWithObjects = "{ \"obj\": { \"field1\": 1, \"field2\": null } }";

        private readonly string InvalidRequest = "";

        private readonly string MalformedRequest = "foo";

        private readonly string EmptyRequest = "{}";

        private readonly string RequestWithQuery =
            $"{{\"query\":\"{QueryString}\"}}";

        private readonly string RequestWithOperation =
            $"{{\"query\":\"{QueryString}\",\"operationName\":\"T\"}}";

        private readonly string RequestWithSerializedVariables =
            $"{{\"query\":\"{QueryString}\",\"variables\":{SerializedVariables},\"operationName\":\"T\"}}";

        private readonly string RequestWithUnserializedVariables =
            $"{{\"query\":\"{QueryString}\",\"variables\":{UnserializedVariables},\"operationName\":\"T\"}}";

        private readonly string RequestWithComplexVariables =
            $"{{\"variables\":{VariablesWithObjects}}}";

    }
}