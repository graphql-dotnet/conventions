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
        public Task<INode> Node(UserContext context, Id id) =>
            context.Get<INode>(id);

        [Description("Retrieve book by its globally unique ID.")]
        public Task<Book> Book(UserContext context, Id id) =>
            context.Get<Book>(id);

        [Description("Retrieve books by their globally unique IDs.")]
        public IEnumerable<Task<Book>> Books(UserContext context, IEnumerable<Id> ids) =>
            ids.Select(context.Get<Book>);

        public Task<Author> Author(UserContext context, Id id) =>
            context.Get<Author>(id);

        [Description("Retrieve authors by their globally unique IDs.")]
        public IEnumerable<Task<Author>> Authors(UserContext context, IEnumerable<Id> ids) =>
            ids.Select(context.Get<Author>);

        [Description("Search for books and authors.")]
        public Connection<SearchResult> Search(
            UserContext userContext,
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
