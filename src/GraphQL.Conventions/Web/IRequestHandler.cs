using System;
using System.Threading.Tasks;

namespace GraphQL.Conventions.Web
{
    public interface IRequestHandler
    {
        Task<Response> ProcessRequestAsync(Request request, IUserContext userContext, IServiceProvider serviceProvider = null);

        Task<Response> ValidateAsync(Request request);

        Task<string> DescribeSchemaAsync(bool returnJson = false, bool includeFieldDescriptions = false, bool includeFieldDeprecationReasons = true);
    }
}
