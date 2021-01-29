# Confluent Kafka Extension: Client Factory
An extension of [Confluent's .NET Client for Apache Kafka<sup>TM</sup>](https://github.com/confluentinc/confluent-kafka-dotnet).

## Installation

* Package Manager
```
Install-Package Confluent.Kafka.FactoryExtension -Version 1.0.1
```

* .NET CLI
```
dotnet add package Confluent.Kafka.FactoryExtension --version 1.0.1
```

* PackageReference
```
<PackageReference Include="Confluent.Kafka.FactoryExtension" Version="1.0.1" />
```

### Features
* Configure multiple Named Kafka Clients using `Microsoft.Extensions.Configuration.IConfiguration`.
* Register Kafka Clients in a thread safe Concurrent Dictionary
* Inject IConsumerFactory and IProducerFactory using `Microsoft.Extensions.DependencyInjection`.

## Usage

Add a global Kafka client:

Take a look in the [examples](examples) directory for example usage.

Microsoft Azure Eventhub example:
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

```c#
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var settings = hostContext.Configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
                    services.TryAddKafkaFactories(settings);

                    services.AddHostedService<Constellation>();
                    services.AddHostedService<Qualification>();
                });
```
