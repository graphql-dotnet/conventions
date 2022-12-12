using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using Tests.Templates;
using Tests.Templates.Extensions;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming

namespace Tests.Adapters
{
    public class BugUnexpectedTypeTests : TestBase
    {
        [Test]
        public async Task Can_Resolve_NonNull_Null_Query()
            => await Can_Resolve_Query_Private<NonNull_Null_Query>();

        [Test]
        public async Task Can_Resolve_Null_NonNull_Query()
            => await Can_Resolve_Query_Private<Null_NonNull_Query>();

        private async Task Can_Resolve_Query_Private<TQuery>()
        {
            var engine = GraphQLEngine
                .New<TQuery>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("query { node { ... on ParentNode {  id, nested { id } } } }")
                .ExecuteAsync();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("node", "id", "UGFyZW50Tm9kZTox");
            result.Data.ShouldHaveFieldWithValue("node", "nested", "id", "Q2hpbGROb2RlOjE=");
        }

        public class NonNull_Null_Query
        {
            public INode Node() => new ParentNode();
            public NonNull<ParentNode> A() => null;
            public ParentNode B() => null;
        }

        public class Null_NonNull_Query
        {
            public INode Node() => new ParentNode();
            public ParentNode A() => null;
            public NonNull<ParentNode> B() => null;
        }
        public class ChildNode : INode
        {
            public Id Id => Id.New<ChildNode>(1);
        }
        public class ParentNode : INode
        {
            public Id Id => Id.New<ParentNode>(1);
            public ChildNode Nested() => new ChildNode();
        }
    }
}
