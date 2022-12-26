namespace GraphQL.Conventions.Tests.Server.Data.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly Dictionary<int, AuthorDto> _authors = new Dictionary<int, AuthorDto>
    {
        {
            1,
            new AuthorDto
            {
                Id = 1,
                FirstName = "Benny",
                LastName = "Frank",
            }
        },
        {
            2,
            new AuthorDto
            {
                Id = 2,
                FirstName = "Amalie",
                LastName = "Jones",
            }
        },
        {
            3,
            new AuthorDto
            {
                Id = 3,
                FirstName = "Stewart",
                LastName = "Clarksen",
            }
        },
        {
            4,
            new AuthorDto
            {
                Id = 4,
                FirstName = "Sony",
                LastName = "Mahony",
            }
        },
    };

    private int _nextId = 1;

    public int AddAuthor(AuthorDto author)
    {
        var id = _nextId++;
        _authors[id] = author;
        _authors[id].Id = id;
        return id;
    }

    public AuthorDto GetAuthorById(int id)
    {
        if (_authors.TryGetValue(id, out var author))
        {
            return author;
        }
        return null;
    }

    public IEnumerable<AuthorDto> GetAuthorsByIds(List<int> ids)
    {
        foreach (var id in ids)
        {
            yield return GetAuthorById(id);
        }
    }

    public IEnumerable<AuthorDto> SearchForAuthorsByLastName(string searchString)
    {
        foreach (var author in _authors.Values)
        {
            if (author.LastName?.ToLower()?.Contains(searchString?.ToLower()) ?? false)
            {
                yield return author;
            }
        }
    }
}
