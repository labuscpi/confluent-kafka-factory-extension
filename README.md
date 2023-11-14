# Confluent Kafka Extension: Client Factory
An extension of [Confluent's .NET Client for Apache Kafka<sup>TM</sup>](https://github.com/confluentinc/confluent-kafka-dotnet).

## Installation

* Package Manager
```
Install-Package Confluent.Kafka.FactoryExtensions -Version 6.0.0
```

* .NET CLI
```
dotnet add package Confluent.Kafka.FactoryExtensions --version 6.0.0
```

* PackageReference
```
<PackageReference Include="Confluent.Kafka.FactoryExtensions" Version="6.0.0" />
```

### Features
* Configure multiple Named Kafka Clients using `Microsoft.Extensions.Configuration.IConfiguration`.
* Register Kafka Clients in a thread safe Concurrent Dictionary.
* Inject IConsumerFactory and IProducerFactory using `Microsoft.Extensions.DependencyInjection`.

## Usage

Take a look in the [examples](examples) directory for example usage.

```json
{
  "KafkaSettings": {
    "Consumers": {
      "Constellation": {
        "Topic": "EVENTHUB",
        "Config": {
          "BootstrapServers": "NAMESPACENAME.servicebus.windows.net:9093",
          "ClientId": null,
          "Debug": null,
          "SaslMechanism": "Plain",
          "SaslUsername": "$ConnectionString",
          "SecurityProtocol": "SaslSsl",
          "SocketKeepaliveEnable": true,
          "SaslPassword": "{YOUR.EVENTHUBS.CONNECTION.STRING}",
          "GroupId": "CONSUMERGROUP",
          "AutoOffsetReset": "Latest",
          "BrokerVersionFallback": "1.0.0"
        }
      },
      "Qualification": {
        "Topic": "EVENTHUB",
        "Config": {
          "BootstrapServers": "NAMESPACENAME.servicebus.windows.net:9093",
          "ClientId": null,
          "Debug": null,
          "SaslMechanism": "Plain",
          "SaslUsername": "$ConnectionString",
          "SecurityProtocol": "SaslSsl",
          "SocketKeepaliveEnable": true,
          "SaslPassword": "{YOUR.EVENTHUBS.CONNECTION.STRING}",
          "GroupId": "CONSUMERGROUP",
          "AutoOffsetReset": "Latest",
          "BrokerVersionFallback": "1.0.0"
        }
      }
    }
  }
}

```
> **_Important:_**  Replace {YOUR.EVENTHUBS.CONNECTION.STRING} with the connection string for your Event Hubs namespace.
> For instructions on getting the connection string,
> see [Get an Event Hubs connection string](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-get-connection-string).
> Here's an example configuration:
> "SaslPassword" : "Endpoint=sb://mynamespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=XXXXXXXXXXXXXXXX";

#### DI Configuration
Add all .json settings files to *IConfigurationBuilder*,
get configuration section `var configuration = hostContext.Configuration.GetSection(nameof(KafkaSettings));`
and register the client factories in DI with `services.TryAddKafkaFactories(configuration);`
```c#
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
                    var configuration = hostContext.Configuration.GetSection(nameof(KafkaSettings));
                    services.TryAddKafkaFactories(configuration);

                    services.AddHostedService<Constellation>();
                    services.AddHostedService<Qualification>();
                });
```
---
* Microsoft Azure Eventhub Consumer example: (`ConsumerService.Example` is example projects)

#### Constructor Injection in `IHostedService`
```c#
        public Constellation(IConsumerFactory factory, ILogger<Constellation> logger)
        {
            _factory = factory;
            _logger = logger;
        }
```

Construct the `ICosumerHandle<TKey, TValue>` from the factory `var handle = _factory.Create<TKey, TValue>(nameof(Constellation));`

```c#
        private IConsumerHandle<TKey, TValue> GetHandle<TKey, TValue>()
        {
            // Create handle on name registered in Configuration, case sensitive
            var handle = _factory.Create<TKey, TValue>(nameof(Constellation));

            // Optional Handler Action Setup
            handle.Builder
                .SetErrorHandler((_, error) => { Log(LogLevel.Error, error.Reason); })
                .SetLogHandler((_, message) => { Log(LogLevel.Information, message.Message); });

            // Available Handler
            // SetStatisticsHandler()
            // SetOffsetsCommittedHandler()
            // SetPartitionsAssignedHandler()
            // SetPartitionsRevokedHandler()
            // SetOAuthBearerTokenRefreshHandler()

            // Available Key and Value Deserializer Setup
            // SetKeyDeserializer()
            // SetValueDeserializer()

            return handle;
        }
```
> **_Note:_** A `CustomConsumerBuilder` object is instantiated. Both Handler Actions
> and Key/Value Deserializer Setup is available.

```c#
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartConsumerLoop<string, string>(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop<TKey, TValue>(CancellationToken cancellationToken)
        {
            var handle = GetHandle<TKey, TValue>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = handle.Consume(cancellationToken);
                    Log(LogLevel.Information, "{0}: {1}", cr.Message.Key, cr.Message.Value);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    Log(LogLevel.Error, "Consume error: {0}", e.Error.Reason);

                    if (e.Error.IsFatal)
                        break;
                }
                catch (Exception e)
                {
                    Log(LogLevel.Error, "Unexpected error: {0}", e.Message);
                    break;
                }
            }
        }
```
