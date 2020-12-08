using System.Collections.Generic;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients;

namespace Confluent.Kafka.FactoryExtension.Models
{
    public sealed class KafkaSettings
    {
        public Dictionary<string, ConsumerSettings> Consumers { get; set; }
        public Dictionary<string, ProducerSettings> Producers { get; set; }
    }
}