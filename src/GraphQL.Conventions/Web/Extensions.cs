using System;
using System.IO;

namespace GraphQL.Conventions.Web
{
    public static class Extensions
    {
        [Obsolete("Please use a GraphQL serialization library such as GraphQL.SystemTextJson to deserialize requests.")]
        public static Request ToGraphQLRequest(this Stream stream) =>
            Request.New(stream);
    }
}
