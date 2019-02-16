using System.Threading.Tasks;

namespace GraphQL.Conventions.Web
{
    public interface IRequestHandler
    {
        Task<Response> ProcessRequest(Request request, IUserContext userContext, IDependencyInjector dependencyInjector = null);

        Response Validate(Request request);

        string DescribeSchema(
            bool returnJson = false,
            bool includeFieldDescriptions = false,
            bool includeFieldDeprecationReasons = true);
    }
}
