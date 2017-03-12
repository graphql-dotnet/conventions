using System.Threading.Tasks;

namespace GraphQL.Conventions.Web
{
    public interface IRequestHandler
    {
        Task<Response> ProcessRequest(Request request, IUserContext userContext);

        Response Validate(Request request);

        string DescribeSchema(bool returnJson = false);
    }
}
