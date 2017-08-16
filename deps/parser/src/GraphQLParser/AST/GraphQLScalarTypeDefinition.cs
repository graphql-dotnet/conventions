using System.Collections.Generic;

namespace GraphQLParser.AST
{
    public class GraphQLScalarTypeDefinition : GraphQLTypeDefinition
    {
        public IEnumerable<GraphQLDirective> Directives { get; set; }

        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.ScalarTypeDefinition;
            }
        }

        public GraphQLName Name { get; set; }
    }
}