namespace GraphQLParser.AST
{
    using System.Collections.Generic;

    public class GraphQLInlineFragment : ASTNode
    {
        public IEnumerable<GraphQLDirective> Directives { get; set; }

        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.InlineFragment;
            }
        }

        public GraphQLSelectionSet SelectionSet { get; set; }
        public GraphQLNamedType TypeCondition { get; set; }
    }
}