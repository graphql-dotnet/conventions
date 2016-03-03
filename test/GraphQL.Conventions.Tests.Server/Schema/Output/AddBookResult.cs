using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Tests.Server.Schema.Types;
using GraphQL.Conventions.Types.Relay;

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
