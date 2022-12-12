using System;
using System.Collections.Generic;

namespace GraphQL.Conventions.Tests.Server.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly Dictionary<int, BookDto> _books = new Dictionary<int, BookDto>
        {
            {
                1,
                new BookDto
                {
                    Id = 1,
                    Title = "A Lone Wolf in the Forest",
                    ReleaseDate = new DateTime(1985, 1, 1),
                    AuthorIds = new List<int> { 1, 2 }
                }
            },
            {
                2,
                new BookDto
                {
                    Id = 2,
                    Title = "Surfer Boy",
                    ReleaseDate = new DateTime(1998, 1, 1),
                    AuthorIds = new List<int> { 2 }
                }
            },
            {
                3,
                new BookDto
                {
                    Id = 3,
                    Title = "GraphQL, a Love Story",
                    ReleaseDate = new DateTime(2015, 1, 1),
                    AuthorIds = new List<int> { 4 }
                }
            },
            {
                4,
                new BookDto
                {
                    Id = 4,
                    Title = "Dare I Say What?",
                    ReleaseDate = new DateTime(2009, 1, 1),
                    AuthorIds = new List<int> { 3, 4 }
                }
            },
        };

        private int _nextId = 5;

        public int AddBook(BookDto book)
        {
            var id = _nextId++;
            _books[id] = book;
            _books[id].Id = id;
            return id;
        }

        public BookDto GetBookById(int id)
        {
            if (_books.TryGetValue(id, out var book))
            {
                return book;
            }
            return null;
        }

        public IEnumerable<BookDto> GetBooksByIds(List<int> ids)
        {
            foreach (var id in ids)
            {
                yield return GetBookById(id);
            }
        }

        public IEnumerable<BookDto> SearchForBooksByTitle(string searchString)
        {
            foreach (var book in _books.Values)
            {
                if (book.Title?.ToLower()?.Contains(searchString?.ToLower()) ?? false)
                {
                    yield return book;
                }
            }
        }
    }
}
