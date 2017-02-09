using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Tests.Server.Schema.Types;

namespace GraphQL.Conventions.Tests.Server.Schema.Output
{
    [Description("The result of an add-book operation.")]
    public class AddBookResult : IRelayMutationOutputObject
    {
        public string ClientMutationId { get; set; }

        [Description("The book added.")]
        public Book Book { get; set; }
    }
}
