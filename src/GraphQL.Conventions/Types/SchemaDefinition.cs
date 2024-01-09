// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public class SchemaDefinition<TQuery>
    {
        public TQuery Query { get; set; }
    }

    public class SchemaDefinitionWithMutation<TMutation>
    {
        public TMutation Mutation { get; set; }
    }

    public class SchemaDefinitionWithSubscription<TSubscription>
    {
        public TSubscription Subscription { get; set; }
    }

    public class SchemaDefinitionWithMutationAndSubscription<TMutation, TSubscription>
    {
        public TMutation Mutation { get; set; }

        public TSubscription Subscription { get; set; }
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
}
