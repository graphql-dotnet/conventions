using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GraphQL.Conventions.Tests.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder
                .UseStartup<Startup>()
                .ConfigureKestrel(options => options.AllowSynchronousIO = true)) // for Newtonsoft.Json support
            .ConfigureLogging(l => l.AddConsole());
    }
}
