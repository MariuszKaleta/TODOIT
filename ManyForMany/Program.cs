using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TODOIT
{
    public class Program
    {
        public static void Main(string[] args)
        {

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                //.UseApplicationInsights()
                .UseConfiguration(new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .Build())
                .UseStartup<Startup>()
                //.ConfigureLogging(logging =>
               // {
              //      logging.ClearProviders();
              //      logging.AddConsole();
              //  })
                .Build();
        }
    }
}