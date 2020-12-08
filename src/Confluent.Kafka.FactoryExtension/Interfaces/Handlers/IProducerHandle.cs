using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka.FactoryExtension.Builders;
using Confluent.Kafka.FactoryExtension.Interfaces.Handlers.Common;

namespace Confluent.Kafka.FactoryExtension.Interfaces.Handlers
{
    public interface IProducerHandle<TKey, TValue> : IClientHandle
    {
        CustomProducerBuilder<TKey, TValue> Builder { get; }
        IProducer<TKey, TValue> Producer { get; }
        
        Message<TKey, TValue> CreateMessage(TKey key, TValue value, Headers headers = null);
        void Produce(Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null);
        void Produce(Partition partition, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null);
        Task<DeliveryResult<TKey, TValue>> ProduceAsync(Message<TKey, TValue> message, CancellationToken cancellationToken = default);
        Task<DeliveryResult<TKey, TValue>> ProduceAsync(Partition partition, Message<TKey, TValue> message, CancellationToken cancellationToken = default);
    }
}