namespace GraphQL.Conventions.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new GraphQL.Conventions.Tests.Web.RequestHandlerTests().Can_Ignore_Types_From_Unwanted_Namespaces();
        }
    }
}