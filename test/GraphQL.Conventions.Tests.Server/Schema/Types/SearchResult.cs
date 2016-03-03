using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Types;

namespace GraphQL.Conventions.Tests.Server.Schema.Types
{
    [Description("A search result")]
    public class SearchResult : Union<Author, Book>
    {
    }
}
