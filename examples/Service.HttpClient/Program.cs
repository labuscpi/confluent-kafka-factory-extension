using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.HttpClient.Models.Settings;
using Service.HttpClient.Services;
using Service.HttpClient.Extensions.ServiceCollection;

namespace Service.HttpClient;

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
            ConsoleWriteLine(e);
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                var configuration = builder.Build();
                var configSubPath = configuration.GetValue<string>("CONFIG_SUB_PATH");
                var directoryContents =
                    context.HostingEnvironment.ContentRootFileProvider.GetDirectoryContents(configSubPath);
                foreach (var file in directoryContents.Where(x => x.Name.EndsWith(".json")))
                    builder.AddJsonFile(file.PhysicalPath, true, false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                services.Configure<ServicesSettings>(configuration.GetSection(ServicesSettings.Key),
                    o => o.BindNonPublicProperties = true);

                services.AddHttpClient(configuration);

                services.AddHostedService<WorkerService>();
            });

    private static void ConsoleWriteLine(Exception e)
        => Console.WriteLine(new
        {
            e.Message,
            e.Source,
            e.StackTrace
        });
}
