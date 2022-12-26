namespace GraphQL.Conventions.Tests.Server.Data.Repositories;

public interface IBookRepository
{
    int AddBook(BookDto book);

    BookDto GetBookById(int id);

    IEnumerable<BookDto> GetBooksByIds(List<int> ids);

    IEnumerable<BookDto> SearchForBooksByTitle(string searchString);
}
