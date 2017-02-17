using System.IO;

namespace GraphQL.Conventions.Web
{
    public static class Extensions
    {
        public static Request ToGraphQLRequest(this Stream stream) =>
            Request.New(stream);

        public static Response ToResponse(this ExecutionResult result, Request request, GraphQLEngine engine) =>
            new Response(request, result, engine.SerializeResult(result));

        public static string IdentifierForTypeOrNull<T>(this Id id) =>
            id.IsIdentifierForType<T>() ? id.IdentifierForType<T>() : null;
    }
}
