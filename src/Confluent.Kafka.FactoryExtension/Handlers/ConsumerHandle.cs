using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka.FactoryExtension.Builders;
using Confluent.Kafka.FactoryExtension.Handlers.Common;
using Confluent.Kafka.FactoryExtension.Interfaces.Handlers;
using Confluent.Kafka.FactoryExtension.Models.Settings.Clients;
using Confluent.Kafka.FactoryExtension.Validators;
using FluentValidation;

namespace Confluent.Kafka.FactoryExtension.Handlers
{
    internal sealed class ConsumerHandle<TKey, TValue> : ClientHandle, IConsumerHandle<TKey, TValue>, IDisposable
    {
        public CustomConsumerBuilder<TKey, TValue> Builder { get; }
        public IConsumer<TKey, TValue> Consumer => Builder.Build();


        public ConsumerHandle(ConsumerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            new ConsumerSettingsValidator().ValidateAndThrow(settings);

            Topic = settings.Topic;

            var config = new ConsumerConfig();
            foreach (var (key, value) in settings.Config.Where(x => !string.IsNullOrEmpty(x.Value)))
                config.Set(key, value);

            Builder = new CustomConsumerBuilder<TKey, TValue>(config);
        }

        public ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout)
            => Subscribe().Consumer.Consume(millisecondsTimeout);

        public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default)
            => Subscribe().Consumer.Consume(cancellationToken);

        public ConsumeResult<TKey, TValue> Consume(TimeSpan timeout)
            => Subscribe().Consumer.Consume(timeout);

        private ConsumerHandle<TKey, TValue> Subscribe()
        {
            if (string.IsNullOrEmpty(Consumer.Subscription?.FirstOrDefault(x => x.Equals(Topic))))
                Consumer.Subscribe(Topic);

            return this;
        }

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