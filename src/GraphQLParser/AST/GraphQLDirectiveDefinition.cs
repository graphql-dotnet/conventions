namespace GraphQLParser.AST
{
    using System.Collections.Generic;

    public class GraphQLDirectiveDefinition : GraphQLTypeDefinition
    {
        public IEnumerable<GraphQLInputValueDefinition> Arguments { get; set; }
        public IEnumerable<GraphQLInputValueDefinition> Definitions { get; set; }

        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.DirectiveDefinition;
            }
        }

        public IEnumerable<GraphQLName> Locations { get; set; }
        public GraphQLName Name { get; set; }
    }
}