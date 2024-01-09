// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions.Relay
{
    [InputType]
    public interface IRelayMutationInputObject
    {
        string ClientMutationId { get; set; }
    }
}
