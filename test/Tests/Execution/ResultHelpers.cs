using GraphQL.Conventions.Tests;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Execution
{
    public static class ResultHelpers
    {
        public static void AssertNoErrorsInResult(string response) =>
            Assert.IsNull(JObject
                .Parse(response)
                .GetValue("errors"));
    }
}
