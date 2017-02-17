using System.Linq;

namespace GraphQL.Conventions.Types.Descriptors.Extensions
{
    public static class DescriptorExtensions
    {
        public static bool HasField(this GraphTypeInfo typeInfo, string fieldName) =>
            typeInfo.Fields.Any(field => field.Name == fieldName);
    }
}
