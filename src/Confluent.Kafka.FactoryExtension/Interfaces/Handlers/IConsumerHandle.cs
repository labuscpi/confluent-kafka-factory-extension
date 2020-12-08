using System;
using System.Threading;
using Confluent.Kafka.FactoryExtension.Builders;

namespace Confluent.Kafka.FactoryExtension.Interfaces.Handlers
{
    public interface IConsumerHandle<TKey, TValue>
    {
        CustomConsumerBuilder<TKey, TValue> Builder { get; }
        IConsumer<TKey, TValue> Consumer { get; }
        
        ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout);
        ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default);
        ConsumeResult<TKey, TValue> Consume(TimeSpan timeout);
    }
}