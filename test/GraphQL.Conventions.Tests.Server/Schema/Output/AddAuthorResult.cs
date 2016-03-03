using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Tests.Server.Schema.Types;
using GraphQL.Conventions.Types.Relay;

namespace GraphQL.Conventions.Tests.Server.Schema.Output
{
    [Description("The result of an add-author operation.")]
    public class AddAuthorResult : IRelayMutationOutputObject
    {
        public string ClientMutationId { get; set; }

        [Description("The author added.")]
        public Author Author { get; set; }
    }
}
