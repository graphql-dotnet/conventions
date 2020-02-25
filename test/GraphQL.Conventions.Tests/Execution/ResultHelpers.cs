﻿using Newtonsoft.Json.Linq;

namespace GraphQL.Conventions.Tests.Execution
{
    public static class ResultHelpers
    {
        public static void AssertNoErrorsInResult(string response) =>
            Assert.IsNull(JObject
                .Parse(response)
                .GetValue("errors"));
    }
}
