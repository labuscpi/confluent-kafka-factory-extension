using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Service.HttpClient.Interfaces.Settings;
using Service.HttpClient.Models.Settings;
using Service.HttpClient.Services;
using Service.HttpClient.Extensions.ServiceCollection;

namespace Service.HttpClient
{
    public class Program
    {
        private static int _upper;
        public static void Main(string[] args)
        {
            try
            {
                if (!int.TryParse(args.FirstOrDefault(), out _upper))
                    _upper = 5;

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
                    var directoryContents = context.HostingEnvironment.ContentRootFileProvider.GetDirectoryContents(configSubPath);
                    foreach (var file in directoryContents.Where(x => x.Name.EndsWith(".json")))
                        builder.AddJsonFile(file.PhysicalPath, true, false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    var settings = configuration.GetSection(nameof(ServicesSettings)).Get<ServicesSettings>();
                    settings.WorkerService.Upper = _upper;
                    services.TryAddSingleton<IServicesSettings>(settings);
                    services.TryAddSingleton<IWorkerServiceSettings>(settings.WorkerService);

                    services.AddHttpClient(settings.WorkerService.HttpClientName, configuration);

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
}
