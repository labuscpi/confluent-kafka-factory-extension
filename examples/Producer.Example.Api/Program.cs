using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Producer.Example.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var configuration = builder.Build();
                    var configSubPath = configuration.GetValue<string>("CONFIG_SUB_PATH");
                    var directoryContents = context.HostingEnvironment.ContentRootFileProvider.GetDirectoryContents(configSubPath);
                    foreach (var file in directoryContents.Where(x => x.Name.EndsWith(".json")))
                        builder.AddJsonFile(file.PhysicalPath, true, false);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
