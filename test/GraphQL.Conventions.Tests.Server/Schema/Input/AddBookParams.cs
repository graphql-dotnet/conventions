using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Relay;

namespace GraphQL.Conventions.Tests.Server.Schema.Input
{
    [Description("Operation for adding a new book to the library.")]
    public class AddBookParams : IRelayMutationInputObject
    {
        public string ClientMutationId { get; set; }

        [Description("The book to add.")]
        public NonNull<BookInput> Book { get; set; }
    }
}
