using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FactoryExtension.Example.Abstractions.Models;
using FactoryExtension.Example.Common.Extensions;
using FactoryExtension.Example.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Producer.Example.Api.Controllers.Common;

namespace Producer.Example.Api.Controllers;

public class WeatherForecastController(
    IProduceHelper<long, string> produceHelper,
    ILogger<WeatherForecastController> logger)
    : ProjectControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetWeatherForecastAsync()
    {
        try
        {
            var key = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var value = GetWeatherForecast().SerializeObject();
            var result = await produceHelper.SendMessageAsync(key, value);

            return new ObjectResult(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[Unexpected error::{Message}]", e.GetMessage());

            return new BadRequestObjectResult(new
            {
                Message = e.GetMessage(),
                e.StackTrace
            });
        }
    }

    private static IEnumerable<WeatherForecast> GetWeatherForecast()
    {
        var rng = new Random();

        return
        [
            .. Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = WeatherForecast.Summaries[rng.Next(WeatherForecast.Summaries.Length)]
                })
        ];
    }
}
