using System.Threading.Tasks;
using GraphQL.Conventions.Execution;

namespace GraphQL.Conventions.Attributes
{
    public interface IExecutionFilterAttribute : IAttribute
    {
        Task<object> Execute(IResolutionContext context, FieldResolutionDelegate next);
    }
}
