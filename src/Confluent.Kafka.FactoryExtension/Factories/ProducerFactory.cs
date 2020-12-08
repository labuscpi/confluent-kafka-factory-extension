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
    public sealed class ProducerFactory : ClientFactory, IProducerFactory
    {
        private readonly IOptionsMonitor<ProducerSettings> _optionsMonitor;

        public ProducerFactory(IOptionsMonitor<ProducerSettings> optionsMonitor)
            => _optionsMonitor = optionsMonitor;

        public IProducerHandle<TKey, TValue> Create<TKey, TValue>(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            SetKeyPrefix(nameof(ProducerHandle<TKey, TValue>));

            return (IProducerHandle<TKey, TValue>) ClientCollection.ActiveHandlers.GetOrAdd(GetClientHandleKey(name), ActiveHandleFactory<TKey, TValue>())
                .Value;
        }

        protected override ClientHandle CreateHandle<TKey, TValue>(string name)
            => new ProducerHandle<TKey, TValue>(GetSettings(name));

        private ProducerSettings GetSettings(string name)
            => _optionsMonitor.Get(name.Replace(KeyPrefix, "", StringComparison.OrdinalIgnoreCase));
    }
}