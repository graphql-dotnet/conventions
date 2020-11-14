using Tests.Web;

namespace Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new RequestHandlerTests().Can_Ignore_Types_From_Unwanted_Namespaces().Wait();
        }
    }
}
