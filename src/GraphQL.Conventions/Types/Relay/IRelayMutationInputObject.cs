namespace GraphQL.Conventions.Relay
{
    [InputType]
    public interface IRelayMutationInputObject
    {
        string ClientMutationId { get; set; }
    }
}