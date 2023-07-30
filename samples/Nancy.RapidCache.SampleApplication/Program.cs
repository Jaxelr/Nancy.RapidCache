using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Asp.Net.Example
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>()
                    .UseIISIntegration();
                })
                .Build();

            host.Run();
        }
    }
}
