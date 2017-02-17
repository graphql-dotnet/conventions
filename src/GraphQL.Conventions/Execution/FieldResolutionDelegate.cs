using System.Threading.Tasks;

namespace GraphQL.Conventions.Execution
{
    public delegate Task<object> FieldResolutionDelegate(IResolutionContext context);
}
