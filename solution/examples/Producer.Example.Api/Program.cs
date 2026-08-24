using System;
using System.Linq;
using Confluent.Kafka.FactoryExtensions.Extensions;
using Confluent.Kafka.FactoryExtensions.Models;
using FactoryExtension.Example.Common.Extensions;
using FactoryExtension.Example.Utilities.Interfaces;
using FactoryExtension.Example.Utilities.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;

var webBuilder = WebApplication.CreateBuilder(args);

var subPath = webBuilder.Configuration.GetValue<string>("CONFIG_SUB_PATH");
var filePaths = webBuilder.Environment.ContentRootFileProvider
    .GetDirectoryContents(subPath)
    .GetJsonFilePaths();

foreach (var file in filePaths)
{
    webBuilder.Configuration.AddJsonFile(file, true, false);
}

// Add services to the container.
var kafkaSettings = webBuilder.Configuration.GetSection(KafkaSettings.Key);
webBuilder.Services.TryAddKafkaFactories(kafkaSettings);
webBuilder.Services.TryAddSingleton<IProduceHelper<long, string>, ProduceHelper<long, string>>();

webBuilder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
webBuilder.Services.AddEndpointsApiExplorer();
webBuilder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Producer.Example.Api",
        Version = "v1"
    });
}).AddSwaggerGenNewtonsoftSupport();

var app = webBuilder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Producer.Example.Api v1"));

app.MapControllers();

await app.RunAsync();
