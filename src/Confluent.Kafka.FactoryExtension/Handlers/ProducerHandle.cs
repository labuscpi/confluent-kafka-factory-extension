using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka.FactoryExtension.Builders;
using Confluent.Kafka.FactoryExtension.Handlers.Common;
using Confluent.Kafka.FactoryExtension.Interfaces.Handlers;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients;

namespace Confluent.Kafka.FactoryExtension.Handlers
{
    internal sealed class ProducerHandle<TKey, TValue> : ClientHandle, IProducerHandle<TKey, TValue>, IDisposable
    {
        [ExcludeFromCodeCoverage] public CustomProducerBuilder<TKey, TValue> Builder { get; }
        [ExcludeFromCodeCoverage] public IProducer<TKey, TValue> Producer => Builder.Build();

        public ProducerHandle(ProducerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            
            Topic = settings.Topic;
            
            var config = new ProducerConfig();
            foreach (var (key, value) in settings.Config.Where(x => !string.IsNullOrEmpty(x.Value)))
                config.Set(key, value);
            
            Builder = new CustomProducerBuilder<TKey, TValue>(config);
        }

        public Message<TKey, TValue> CreateMessage(TKey key, TValue value, Headers headers = null)
        {
            var dateTime = DateTimeOffset.UtcNow;
            var timestamp = new Timestamp(dateTime);

            return new Message<TKey, TValue>
            {
                Key = key,
                Value = value,
                Timestamp = timestamp,
                Headers = headers
            };
        }

        [ExcludeFromCodeCoverage]
        public void Produce(Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
            => Producer.Produce(Topic, message, deliveryHandler);

        [ExcludeFromCodeCoverage]
        public void Produce(Partition partition, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
            => Producer.Produce(CreateTopicPartition(partition), message, deliveryHandler);

        [ExcludeFromCodeCoverage]
        public Task<DeliveryResult<TKey, TValue>> ProduceAsync(Message<TKey, TValue> message, CancellationToken cancellationToken = default)
            => Producer.ProduceAsync(Topic, message, cancellationToken);

        [ExcludeFromCodeCoverage]
        public Task<DeliveryResult<TKey, TValue>> ProduceAsync(Partition partition, Message<TKey, TValue> message,
            CancellationToken cancellationToken = default)
            => Producer.ProduceAsync(CreateTopicPartition(partition), message, cancellationToken);

        [ExcludeFromCodeCoverage]
        private TopicPartition CreateTopicPartition(Partition partition)
            => new TopicPartition(Topic, partition);


        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            // Block until all outstanding produce requests have completed (with or without error).
            try
            {
                Producer?.Flush();
                Producer?.Dispose();
            }
            catch (Exception e)
            {
                if (e is TaskCanceledException)
                    return;
        
                throw;
            }
        }
    }
}