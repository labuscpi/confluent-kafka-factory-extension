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
    internal sealed class ConsumerHandle<TKey, TValue> : ClientHandle, IConsumerHandle<TKey, TValue>, IDisposable
    {
        [ExcludeFromCodeCoverage] public CustomConsumerBuilder<TKey, TValue> Builder { get; }
        [ExcludeFromCodeCoverage] public IConsumer<TKey, TValue> Consumer => Builder.Build();


        public ConsumerHandle(ConsumerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Topic = settings.Topic;

            var config = new ConsumerConfig();
            foreach (var (key, value) in settings.Config.Where(x => !string.IsNullOrEmpty(x.Value)))
                config.Set(key, value);

            Builder = new CustomConsumerBuilder<TKey, TValue>(config);
        }

        [ExcludeFromCodeCoverage]
        public ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout)
            => Subscribe().Consumer.Consume(millisecondsTimeout);

        [ExcludeFromCodeCoverage]
        public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default)
            => Subscribe().Consumer.Consume(cancellationToken);

        [ExcludeFromCodeCoverage]
        public ConsumeResult<TKey, TValue> Consume(TimeSpan timeout)
            => Subscribe().Consumer.Consume(timeout);

        [ExcludeFromCodeCoverage]
        private ConsumerHandle<TKey, TValue> Subscribe()
        {
            if (string.IsNullOrEmpty(Consumer.Subscription?.FirstOrDefault(x => x.Equals(Topic))))
                Consumer.Subscribe(Topic);

            return this;
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            try
            {
                Consumer?.Close();
                Consumer?.Dispose();
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