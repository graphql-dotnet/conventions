using System.Collections.Generic;

namespace GraphQLParser.AST
{
    public class GraphQLObjectValue : GraphQLValue
    {
        public IEnumerable<GraphQLObjectField> Fields { get; set; }

        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.ObjectValue;
            }
        }
    }
}