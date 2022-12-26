namespace GraphQL.Conventions.Tests.Server.Data;

public class BookDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public List<int> AuthorIds { get; set; }

    public DateTime? ReleaseDate { get; set; }
}
