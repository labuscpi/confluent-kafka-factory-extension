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
    public sealed class ConsumerFactory : ClientFactory, IConsumerFactory
    {
        private readonly IOptionsMonitor<ConsumerSettings> _optionsMonitor;

        public ConsumerFactory(IOptionsMonitor<ConsumerSettings> optionsMonitor)
            => _optionsMonitor = optionsMonitor;

        public IConsumerHandle<TKey, TValue> Create<TKey, TValue>(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            SetKeyPrefix(nameof(ConsumerHandle<TKey, TValue>));

            return (IConsumerHandle<TKey, TValue>) ClientCollection.ActiveHandlers
                .GetOrAdd(GetClientHandleKey(name), ActiveHandleFactory<TKey, TValue>()).Value;
        }

        protected override ClientHandle CreateHandle<TKey, TValue>(string name)
            => new ConsumerHandle<TKey, TValue>(GetSettings(name));

        private ConsumerSettings GetSettings(string name)
            => _optionsMonitor.Get(name.Replace(KeyPrefix, "", StringComparison.OrdinalIgnoreCase));
    }
}