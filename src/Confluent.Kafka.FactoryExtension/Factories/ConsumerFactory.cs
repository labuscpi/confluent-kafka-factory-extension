using System;
using Confluent.Kafka.FactoryExtension.Factories.Common;
using Confluent.Kafka.FactoryExtension.Handlers;
using Confluent.Kafka.FactoryExtension.Handlers.Common;
using Confluent.Kafka.FactoryExtension.Interfaces.Factories;
using Confluent.Kafka.FactoryExtension.Interfaces.Handlers;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients;
using Microsoft.Extensions.Options;

namespace Confluent.Kafka.FactoryExtension.Factories
{
    public sealed class ConsumerFactory : ClientFactory<ConsumerSettings>, IConsumerFactory
    {
        public ConsumerFactory(IOptionsMonitor<ConsumerSettings> optionsMonitor) : base(optionsMonitor)
        {
        }

        public IConsumerHandle<TKey, TValue> Create<TKey, TValue>(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            SetKeyPrefix(GetType());

            return (IConsumerHandle<TKey, TValue>) ClientCollection.Handles
                .GetOrAdd(GetClientHandleKey(name), HandleFactory<TKey, TValue>()).Value;
        }

        protected override ClientHandle CreateHandle<TKey, TValue>(string name)
            => new ConsumerHandle<TKey, TValue>(GetSettings(name));
    }
}