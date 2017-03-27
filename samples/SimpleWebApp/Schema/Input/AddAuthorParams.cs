using GraphQL.Conventions.Relay;

namespace GraphQL.Conventions.Tests.Server.Schema.Input
{
    [Description("Operation for adding a new author.")]
    public class AddAuthorParams : IRelayMutationInputObject
    {
        public string ClientMutationId { get; set; }

        [Description("The author to add.")]
        public NonNull<AuthorInput> Author { get; set; }
    }
}
