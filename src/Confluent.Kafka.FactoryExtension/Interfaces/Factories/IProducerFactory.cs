using Confluent.Kafka.FactoryExtension.Interfaces.Handlers;

namespace Confluent.Kafka.FactoryExtension.Interfaces.Factories
{
    public interface IProducerFactory
    {
        IProducerHandle<TKey, TValue> Create<TKey, TValue>(string name);
    }
}