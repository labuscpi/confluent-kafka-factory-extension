using Confluent.Kafka.FactoryExtension.Interfaces.Handlers;

namespace Confluent.Kafka.FactoryExtension.Interfaces.Factories
{
    public interface IConsumerFactory
    {
        IConsumerHandle<TKey, TValue> Create<TKey, TValue>(string name);
    }
}