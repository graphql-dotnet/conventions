namespace GraphQL.Conventions
{
    public class SchemaDefinition<TQuery>
    {
        public TQuery Query { get; set; }
    }

    public class SchemaDefinitionWithMutation<TMutation>
        : SchemaDefinition<Query, TMutation>
    {
    }

    public class SchemaDefinitionWithSubscription<TSubscription>
    {
        public Query Query { get; set; }

        public TSubscription Subscription { get; set; }
    }

    public class SchemaDefinitionWithQueryAndQueryExtensions<TQuery, TQueryExtensions>
    {
        public TQuery Query { get; set; }
        public TQueryExtensions QueryExtensions { get; set; }
    }

    public class SchemaDefinitionWithMutationAndSubscription<TMutation, TSubscription>
        : SchemaDefinition<Query, TMutation, TSubscription>
    {
    }

    public class SchemaDefinition<TQuery, TMutation>
    {
        public TQuery Query { get; set; }

        public TMutation Mutation { get; set; }
    }

    public class SchemaDefinition<TQuery, TMutation, TSubscription>
    {
        public TQuery Query { get; set; }

        public TMutation Mutation { get; set; }

        public TSubscription Subscription { get; set; }
    }

    public class Query
    {
    }
}
