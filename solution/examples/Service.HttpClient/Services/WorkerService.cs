using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.HttpClient.Interfaces.Settings;

namespace Service.HttpClient.Services
{
    public class WorkerService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWorkerServiceSettings _settings;
        private readonly ILogger<WorkerService> _logger;

        public WorkerService(IHttpClientFactory httpClientFactory, IWorkerServiceSettings settings, ILogger<WorkerService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(async () => await StartLoopAsync(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private async Task StartLoopAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

                if (stoppingToken.IsCancellationRequested)
                    stoppingToken.ThrowIfCancellationRequested();

                var cw = new Stopwatch();
                cw.Start();
                await SendAsync(stoppingToken);
                cw.Stop();

                _logger.LogInformation("Elapsed Milliseconds: {ElapsedMilliseconds}", cw.ElapsedMilliseconds);

                await Task.Delay(TimeSpan.FromMilliseconds(1000), stoppingToken);

                break;
            }
        }

        private async Task SendAsync(CancellationToken stoppingToken)
        {
            try
            {
                var list = new List<Task<HttpResponseMessage>>();
                for (var i = 0; i < _settings.Upper; i++)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    list.Add(_httpClientFactory.CreateClient(_settings.HttpClientName).SendAsync(CreateRequestMessage(), stoppingToken));
                }

                var result = await Task.WhenAll(list);

                var successStatus = result
                    .Where(x => x != null)
                    .Count(x => x.IsSuccessStatusCode);

                _logger.LogInformation("Total message sent: {Upper}", _settings.Upper);
                _logger.LogInformation("Total success message: {SuccessStatus}", successStatus);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        private static HttpRequestMessage CreateRequestMessage()
            => new HttpRequestMessage(HttpMethod.Get, "WeatherForecast");

        private void LogException(Exception e)
            => _logger.LogError(e, "{Message}", e.Message);
    }
}
