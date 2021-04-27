using System;
using GraphQL.Language.AST;

namespace GraphQL.Conventions.Adapters.Types
{
    public class IdGraphType : GraphQL.Types.IdGraphType
    {
        /// <inheritdoc/>
        public override object ParseValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            return new Id(value.ToString());
        }
    }
}
