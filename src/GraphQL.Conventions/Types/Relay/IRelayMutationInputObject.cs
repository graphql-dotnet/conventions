using GraphQL.Conventions.Attributes.MetaData;

namespace GraphQL.Conventions.Types.Relay
{
    [InputType]
    public interface IRelayMutationInputObject
    {
        string ClientMutationId { get; set; }
    }
}