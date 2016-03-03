using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Attributes.MetaData.Relay;
using GraphQL.Conventions.Tests.Server.Data.Repositories;
using GraphQL.Conventions.Tests.Server.Schema.Types;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Relay;

namespace GraphQL.Conventions.Tests.Server.Schema
{
    [ImplementViewer(OperationType.Query)]
    public class Query
    {
        public async Task<INode> Node(
            [Inject] IBookRepository bookRepository,
            [Inject] IAuthorRepository authorRepository,
            Id id)
        {
            await Task.Delay(10); // no-op
            if (id.IsIdentifierForType<Book>())
            {
                return Book(bookRepository, id);
            }
            if (id.IsIdentifierForType<Author>())
            {
                return Author(authorRepository, id);
            }
            return null;
        }

        [Description("Retrieve book by its globally unique ID.")]
        public Book Book(
            [Inject] IBookRepository bookRepository,
            Id id)
        {
            return new Book(bookRepository.GetBookById(int.Parse(id.IdentifierForType<Book>())));
        }

        [Description("Retrieve books by their globally unique IDs.")]
        public IEnumerable<Book> Books(
            [Inject] IBookRepository bookRepository,
            IEnumerable<Id> ids)
        {
            var internalIds = ids
                .Select(id => int.Parse(id.IdentifierForType<Book>()))
                .ToList();
            foreach (var book in bookRepository.GetBooksByIds(internalIds))
            {
                yield return new Book(book);
            }
        }

        public Author Author(
            [Inject] IAuthorRepository authorRepository,
            Id id)
        {
            return new Author(authorRepository.GetAuthorById(int.Parse(id.IdentifierForType<Author>())));
        }

        [Description("Retrieve authors by their globally unique IDs.")]
        public IEnumerable<Author> Authors(
            [Inject] IAuthorRepository authorRepository,
            IEnumerable<Id> ids)
        {
            var internalIds = ids
                .Select(id => int.Parse(id.IdentifierForType<Book>()))
                .ToList();
            foreach (var author in authorRepository.GetAuthorsByIds(internalIds))
            {
                yield return new Author(author);
            }
        }

        [Description("Search for books and authors.")]
        public Connection<SearchResult> Search(
            [Inject] IBookRepository bookRepository,
            [Inject] IAuthorRepository authorRepository,
            [Description("Title or last name.")] NonNull<string> forString)
        {
            var results = new List<SearchResult>();

            foreach (var book in bookRepository.SearchForBooksByTitle(forString.Value))
            {
                results.Add(new SearchResult { Instance = new Book(book) });
            }
            foreach (var author in authorRepository.SearchForAuthorsByLastName(forString.Value))
            {
                results.Add(new SearchResult { Instance = new Author(author) });
            }

            return new Connection<SearchResult>
            {
                TotalCount = results.Count,
                PageInfo = new PageInfo
                {
                    HasNextPage = results.Count > 5,
                    HasPreviousPage = false,
                    StartCursor = Cursor.New<SearchResult>(1),
                    EndCursor = Cursor.New<SearchResult>(Math.Min(5, Math.Max(1, results.Count)))
                },
                Edges = results.Take(5).Select((result, index) => new Edge<SearchResult>
                {
                    Cursor = Cursor.New<SearchResult>(index + 1),
                    Node = result,
                }),
            };
        }
    }
}
