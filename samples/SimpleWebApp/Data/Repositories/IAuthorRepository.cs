namespace GraphQL.Conventions.Tests.Server.Data.Repositories;

public interface IAuthorRepository
{
    int AddAuthor(AuthorDto author);

    AuthorDto GetAuthorById(int id);

    IEnumerable<AuthorDto> GetAuthorsByIds(List<int> ids);

    IEnumerable<AuthorDto> SearchForAuthorsByLastName(string searchString);
}
