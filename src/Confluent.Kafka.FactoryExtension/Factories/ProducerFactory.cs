using System;
using Confluent.Kafka.FactoryExtension.Collections;
using Confluent.Kafka.FactoryExtension.Factories.Common;
using Confluent.Kafka.FactoryExtension.Handlers;
using Confluent.Kafka.FactoryExtension.Handlers.Common;
using Confluent.Kafka.FactoryExtension.Interfaces.Factories;
using Confluent.Kafka.FactoryExtension.Interfaces.Handlers;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients;
using Microsoft.Extensions.Options;

namespace Confluent.Kafka.FactoryExtension.Factories
{
    public sealed class ProducerFactory : ClientFactory<ProducerSettings>, IProducerFactory
    {
        public ProducerFactory(IOptionsMonitor<ProducerSettings> optionsMonitor) : base(optionsMonitor)
        {
        }

        public IProducerHandle<TKey, TValue> Create<TKey, TValue>(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            SetKeyPrefix(GetType());

            return (IProducerHandle<TKey, TValue>) ClientCollection.Instance.Handles
                .GetOrAdd(GetClientHandleKey(name), HandleFactory<TKey, TValue>())
                .Value;
        }

        protected override ClientHandle CreateHandle<TKey, TValue>(string name)
            => new ProducerHandle<TKey, TValue>(GetSettings(name));
    }
}