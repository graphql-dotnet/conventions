namespace GraphQL.Conventions.Attributes.MetaData.Utilities
{
    public interface INameNormalizer
    {
        string AsTypeName(string name);

        string AsFieldName(string name);

        string AsArgumentName(string name);

        string AsEnumMemberName(string name);
    }
}