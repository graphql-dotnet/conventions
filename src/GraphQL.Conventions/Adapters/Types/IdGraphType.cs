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

        public override object Serialize(object value)
            => value is Id idValue ? idValue.ToString() : base.Serialize(value);
    }
}
