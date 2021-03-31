using System;
using GraphQL.Language.AST;

namespace GraphQL.Conventions.Adapters.Types
{
    public class IdGraphType : GraphQL.Types.IdGraphType
    {
        /// <inheritdoc/>
        public override object ParseValue(object value) => value != null 
            ? new Id(value.ToString()) 
            : null;
    }
}