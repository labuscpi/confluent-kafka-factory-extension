using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using FactoryExtension.Example.Abstractions.Models;
using FactoryExtension.Services.Example.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProducerApi.Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ProducerMessageCollection _collection;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ProducerMessageCollection collection, ILogger<WeatherForecastController> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<DeliveryResult<Null, string>> GetWeatherForecastAsync()
        {
            try
            {
                var result = _collection.EnqueueMessage(GetWeatherForecast());

                return new ActionResult<DeliveryResult<Null, string>>(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
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