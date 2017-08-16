using System.Collections.Generic;

namespace GraphQLParser.AST
{
    public class GraphQLDocument : ASTNode
    {
        public IEnumerable<ASTNode> Definitions { get; set; }

        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.Document;
            }
        }
    }
}