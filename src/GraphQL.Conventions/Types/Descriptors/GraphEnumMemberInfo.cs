using System.Reflection;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Types.Descriptors
{
    public class GraphEnumMemberInfo : GraphFieldInfo
    {
        public GraphEnumMemberInfo(ITypeResolver typeResolver, MemberInfo enumValue = null)
            : base(typeResolver, enumValue)
        {
        }
    }
}
