using System.Threading.Tasks;

namespace GraphQL.Conventions.Adapters.Engine.ErrorTransformations
{
    public interface IErrorTransformation
    {
        Task<ExecutionErrors> Transform(ExecutionErrors errors);
    }
}
