#region Copyright

// Copyright 2021. labuscpi
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka.FactoryExtensions.Builders;
using Confluent.Kafka.FactoryExtensions.Handlers.Common;
using Confluent.Kafka.FactoryExtensions.Interfaces.Handlers;
using Confluent.Kafka.FactoryExtensions.Models.Settings.Clients;

namespace Confluent.Kafka.FactoryExtensions.Handlers;

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
    public void Produce(Partition partition, Message<TKey, TValue> message,
        Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
        => Producer.Produce(CreateTopicPartition(partition), message, deliveryHandler);

    [ExcludeFromCodeCoverage]
    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(Message<TKey, TValue> message,
        CancellationToken cancellationToken = default)
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