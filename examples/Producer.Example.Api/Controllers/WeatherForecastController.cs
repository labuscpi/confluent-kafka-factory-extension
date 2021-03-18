using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confluent.Kafka;
using FactoryExtension.Example.Abstractions.Interfaces;
using FactoryExtension.Example.Abstractions.Models;
using FactoryExtension.Example.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Producer.Example.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IProduceHelperIngest _collection;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IProduceHelperIngest collection, ILogger<WeatherForecastController> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult GetWeatherForecastAsync()
        {
            try
            {
                return SendWeatherForecast<Null, string>(GetWeatherForecast().SerializeObject());
            }
            catch (Exception e)
            {
                const string message = "Unexpected error: {0}";
                _logger.LogError(e, message, e.GetMessage());

                return new BadRequestObjectResult(new
                {
                    Message = e.GetMessage(),
                    e.StackTrace
                });
            }
        }

        private ObjectResult SendWeatherForecast<TKey, TValue>(TValue value)
        {
            try
            {
                var message = new Message<TKey, TValue>
                {
                    Value = value,
                    Timestamp = Timestamp.Default
                };
            
                var key = _collection.EnqueueMessage(message, true);

                var report = _collection.GetDeliveryResult<TKey, TValue>(key);
                
                return new OkObjectResult(report);
            }
            catch (OperationCanceledException oce)
            {
                return new BadRequestObjectResult(new
                {
                    Message = oce.GetMessage(),
                    oce.CancellationToken,
                    oce.StackTrace
                });
            }
        }

        private static IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            var rng = new Random();

            return Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = WeatherForecast.Summaries[rng.Next(WeatherForecast.Summaries.Length)]
                })
                .ToList();
        }
    }
}