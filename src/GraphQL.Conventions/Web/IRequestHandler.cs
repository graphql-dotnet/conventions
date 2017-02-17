using System.Threading.Tasks;

namespace GraphQL.Conventions.Web
{
    public interface IRequestHandler
    {
        Task<Response> ProcessRequest(Request request, IUserContext userContext);

        string DescribeSchema(bool returnJson = false);
    }
}
