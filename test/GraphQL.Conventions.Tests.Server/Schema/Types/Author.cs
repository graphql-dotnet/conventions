using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Tests.Server.Data;

namespace GraphQL.Conventions.Tests.Server.Schema.Types
{
    [Description("An author")]
    public class Author : INode
    {
        private readonly AuthorDto _dto;

        public Author(AuthorDto dto)
        {
            _dto = dto;
        }

        [Description("A unique author identifier.")]
        public Id Id => Id.New<Author>(_dto.Id);

        [Description("The author's firstname.")]
        public string FirstName => _dto.FirstName;

        [Description("The author's lastname.")]
        public NonNull<string> LastName => _dto.LastName;
    }
}
